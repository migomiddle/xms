using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.QueryView;
using Xms.QueryView.Abstractions;
using Xms.RibbonButton;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Entity;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Extensions;
using Xms.Security.Abstractions;
using Xms.Security.Principal;
using Xms.Solution.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;
using Xms.Web.Models;
using Xms.WebResource;

namespace Xms.Web.Api
{
    /// <summary>
    /// 视图接口
    /// </summary>
    [Route("{org}/api/schema/queryview")]
    public class QueryViewController : ApiControllerBase
    {
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IQueryViewUpdater _queryViewUpdater;
        private readonly IEntityFinder _entityFinder;
        private readonly IQueryMetadataFinder _queryMetadataFinder;
        private readonly IRibbonButtonFinder _ribbonbuttonFinder;
        private readonly IWebResourceFinder _webResourceFinder;
        private readonly IWebResourceContentCoder _webResourceContentCoder;
        private readonly ISystemUserPermissionService _systemUserPermissionService;

        public QueryViewController(IWebAppContext appContext
            , IQueryViewFinder queryViewFinder
            , IQueryViewUpdater queryViewUpdater
            , IEntityFinder entityService
            , IQueryMetadataFinder queryMetadataFinder
            , IRibbonButtonFinder ribbonbuttonFinder
            , IWebResourceFinder webResourceFinder
            , IWebResourceContentCoder webResourceContentCoder
            , ISystemUserPermissionService systemUserPermissionService)
            : base(appContext)
        {
            _queryViewFinder = queryViewFinder;
            _queryViewUpdater = queryViewUpdater;
            _entityFinder = entityService;
            _queryMetadataFinder = queryMetadataFinder;
            _ribbonbuttonFinder = ribbonbuttonFinder;
            _webResourceFinder = webResourceFinder;
            _webResourceContentCoder = webResourceContentCoder;
            _systemUserPermissionService = systemUserPermissionService;
        }

        /// <summary>
        /// 查询视图
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Description("查询视图")]
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            var authorizedView = _queryViewFinder.QueryAuthorized(n => n.Where(f => f.QueryViewId == id && f.StateCode == RecordState.Enabled));
            var queryView = authorizedView?.First();//_queryViewFinder.FindById(id);
            if (queryView == null)
            {
                return NotFound();
            }
            return JOk(queryView);
        }

        /// <summary>
        /// 查询视图
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="authorized"></param>
        /// <returns></returns>
        [Description("查询视图")]
        [HttpGet("GetByEntityId")]
        public IActionResult GetByEntityId(Guid entityId, bool? authorized)
        {
            List<QueryView.Domain.QueryView> result = null;
            if (authorized.HasValue && authorized.Value)
            {
                result = _queryViewFinder.QueryAuthorized(n => n.Where(f => f.EntityId == entityId && f.StateCode == RecordState.Enabled)
                    .Sort(s => s.SortAscending(f => f.Name)));
            }
            else
            {
                result = _queryViewFinder.FindByEntityId(entityId);
            }
            return JOk(result);
        }

        /// <summary>
        /// 查询视图
        /// </summary>
        /// <param name="entityId"></param>
        /// <param name="authorized"></param>
        /// <returns></returns>
        [Description("查询视图")]
        [HttpGet("GetByEntityName")]
        public IActionResult GetByEntityName(string entityName, bool? authorized)
        {
            List<QueryView.Domain.QueryView> result = null;
            if (authorized.HasValue && authorized.Value)
            {
                result = _queryViewFinder.QueryAuthorized(n => n.Where(f => f.EntityName == entityName && f.StateCode == RecordState.Enabled)
                    .Sort(s => s.SortAscending(f => f.Name)));
            }
            else
            {
                result = _queryViewFinder.FindByEntityName(entityName);
            }
            return JOk(result);
        }

        /// <summary>
        /// 查询视图字段
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Description("查询视图字段")]
        [HttpGet("GetAttributes/{id}")]
        public IActionResult GetAttributes(Guid id)
        {
            var queryView = _queryViewFinder.FindById(id);
            if (queryView == null)
            {
                return NotFound();
            }
            var queryExp = new QueryExpression().DeserializeFromJson(queryView.FetchConfig);
            var result = _queryMetadataFinder.GetAttributes(queryExp);
            return JOk(result);
        }

        [Description("查询视图按钮")]
        [HttpGet("GetButtons/{id}")]
        public IActionResult GetButtons(Guid id)
        {
            var queryView = _queryViewFinder.FindById(id);
            if (queryView == null)
            {
                return NotFound();
            }
            FilterContainer<RibbonButton.Domain.RibbonButton> buttonFilter = FilterContainerBuilder.Build<RibbonButton.Domain.RibbonButton>()
                    .And(w => w.StateCode == RecordState.Enabled && w.EntityId == queryView.EntityId
                        && (w.ShowArea == RibbonButtonArea.ListHead || w.ShowArea == RibbonButtonArea.ListRow));
            var buttons = _ribbonbuttonFinder.Find(queryView.EntityId, null);
            if (queryView.IsCustomButton && queryView.CustomButtons.IsNotEmpty())
            {
                List<Guid> buttonid = new List<Guid>();
                buttonid = buttonid.DeserializeFromJson(queryView.CustomButtons);
                buttons.RemoveAll(x => !buttonid.Contains(x.RibbonButtonId));
            }
            if (buttons.NotEmpty())
            {
                buttons = buttons.OrderBy(x => x.DisplayOrder).ToList();
            }
            return JOk(buttons);
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _queryViewFinder.QueryPaged(x => x.Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.QueryViewId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = QueryViewDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
                return JOk(new PagedList<SolutionComponentItem>()
                {
                    CurrentPage = model.Page
                    ,
                    ItemsPerPage = model.PageSize
                    ,
                    Items = result
                    ,
                    TotalItems = data.TotalItems
                    ,
                    TotalPages = data.TotalPages
                });
            }
            return JOk(data);
        }

        [Description("查询视图权限资源")]
        [HttpGet("PrivilegeResource")]
        public IActionResult PrivilegeResource(bool? authorizationEnabled)
        {
            FilterContainer<QueryView.Domain.QueryView> filter = FilterContainerBuilder.Build<QueryView.Domain.QueryView>();
            filter.And(x => x.StateCode == RecordState.Enabled && x.IsDefault == false && x.IsPrivate == false);
            if (authorizationEnabled.HasValue)
            {
                filter.And(x => x.AuthorizationEnabled == authorizationEnabled.Value);
            }
            var data = _queryViewFinder.Query(x => x.Select(s => new { s.QueryViewId, s.Name, s.EntityId, s.AuthorizationEnabled }).Where(filter));
            if (data.NotEmpty())
            {
                var result = new List<PrivilegeResourceItem>();
                var entities = _entityFinder.FindAll()?.OrderBy(x => x.LocalizedName).ToList();
                foreach (var item in entities)
                {
                    var views = data.Where(x => x.EntityId == item.EntityId);
                    if (!views.Any())
                    {
                        continue;
                    }
                    var group1 = new PrivilegeResourceItem
                    {
                        Label = item.LocalizedName,
                        Children = views.Select(x => (new PrivilegeResourceItem { Id = x.QueryViewId, Label = x.Name, AuthorizationEnabled = x.AuthorizationEnabled })).ToList()
                    };
                    result.Add(group1);
                }
                return JOk(result);
            }
            return JOk();
        }

        [Description("启用视图权限")]
        [HttpPost("AuthorizationEnabled")]
        public IActionResult AuthorizationEnabled(UpdateAuthorizationStateModel model)
        {
            var authorizationViews = _queryViewFinder.Query(x => x.Where(w => w.AuthorizationEnabled == true));
            if (authorizationViews.NotEmpty())
            {
                _queryViewUpdater.UpdateAuthorization(false, authorizationViews.Select(x => x.QueryViewId).ToArray());
            }
            if (Arguments.HasValue(model.ObjectId))
            {
                _queryViewUpdater.UpdateAuthorization(true, model.ObjectId);
            }
            return SaveSuccess();
        }

        /// <summary>
        /// 查询视图
        /// </summary>
        /// <param name="id"></param>
        /// <param name="entityId"></param>
        /// <param name="entityName"></param>
        /// <returns></returns>
        [Description("查询视图信息")]
        [HttpGet("GetViewInfo")]
        public IActionResult GetViewInfo(Guid? id, Guid? entityId, string entityName)
        {
            //视图信息
            List<QueryView.Domain.QueryView> views = null;
            //字段信息
            List<Schema.Domain.Attribute> attributes = null;
            //按钮信息
            List<RibbonButton.Domain.RibbonButton> buttons = null;
            //Web资源信息
            string webResources = null;
            List<Guid> webResourceIds = null;
            QueryView.Domain.QueryView queryView = null;

            //查询视图信息
            if (id.HasValue && !id.Value.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindById(id.Value);
                if (queryView != null)
                {
                    views = new List<QueryView.Domain.QueryView>
                    {
                        queryView
                    };
                }
            }
            else if (entityId.HasValue && !entityId.Equals(Guid.Empty))
            {
                views = _queryViewFinder.QueryAuthorized(n => n.Where(f => f.EntityId == entityId && f.StateCode == RecordState.Enabled)
                        .Sort(s => s.SortAscending(f => f.Name)));
                if (views.NotEmpty())
                {
                    queryView = views.First(x => x.IsDefault);
                }
            }
            else if (entityName.IsNotEmpty())
            {
                views = _queryViewFinder.QueryAuthorized(n => n.Where(f => f.EntityName == entityName && f.StateCode == RecordState.Enabled)
                        .Sort(s => s.SortAscending(f => f.Name)));
                if (views.NotEmpty())
                {
                    queryView = views.FirstOrDefault(x => x.IsDefault);
                }
            }
            if (queryView == null)
            {
                return NotFound();
            }
            //查询字段
            var queryExp = new QueryExpression().DeserializeFromJson(queryView.FetchConfig);
            attributes = _queryMetadataFinder.GetAttributes(queryExp);
            //查询按钮
            FilterContainer<RibbonButton.Domain.RibbonButton> buttonFilter = FilterContainerBuilder.Build<RibbonButton.Domain.RibbonButton>()
                  .And(w => w.StateCode == RecordState.Enabled && w.EntityId == queryView.EntityId
                      && (w.ShowArea == RibbonButtonArea.ListHead || w.ShowArea == RibbonButtonArea.ListRow || w.ShowArea == RibbonButtonArea.SubGrid));
            if (queryView.IsCustomButton && queryView.CustomButtons.IsNotEmpty())
            {
                List<Guid> buttonid = new List<Guid>();
                buttonid = buttonid.DeserializeFromJson(queryView.CustomButtons);
                buttonFilter.And(w => w.RibbonButtonId.In(buttonid));
            }
            buttons = _ribbonbuttonFinder.Query(n => n
           .Where(buttonFilter)
           .Sort(s => s.SortAscending(f => f.DisplayOrder)));
            //查询Web资源信息
            if (queryView.LayoutConfig.IsNotEmpty())
            {
                QueryViewLayoutConfigModel layoutConfig = new QueryViewLayoutConfigModel().DeserializeFromJson(queryView.LayoutConfig);
                if (layoutConfig != null && layoutConfig.ClientResources.NotEmpty())
                {
                    webResourceIds = layoutConfig.ClientResources;
                }
            }
            if (buttons.NotEmpty())
            {
                buttons.ForEach(x =>
                {
                    if (x.JsLibrary != null)
                    {
                        string[] arr = x.JsLibrary.Split(":");
                        if (arr.Length > 1)
                        {
                            Guid webResourceId = Guid.Empty;
                            if (Guid.TryParse(arr[1], out webResourceId))
                            {
                                if (webResourceIds == null)
                                    webResourceIds = new List<Guid>();
                                webResourceIds.Add(webResourceId);
                            }
                        }
                    }
                });
            }
            if (webResourceIds.NotEmpty())
            {
                StringBuilder content = new StringBuilder();
                var result = _webResourceFinder.FindByIds(webResourceIds.ToArray());
                foreach (var item in result)
                {
                    content.Append(_webResourceContentCoder.CodeDecode(item.Content));
                }
                webResources = content.ToString();
            }
            else
            {
                webResources = T["notfound_record"];
            }
            List<Guid> noneReadFields = new List<Guid>();
            //获取字段权限
            if (!CurrentUser.IsSuperAdmin && attributes.Count(n => n.AuthorizationEnabled) > 0)
            {
                var securityFields = attributes.Where(n => n.AuthorizationEnabled).Select(f => f.AttributeId)?.ToList();
                if (securityFields.NotEmpty())
                {
                    //无权限的字段
                    noneReadFields = _systemUserPermissionService.GetNoneReadFields(CurrentUser.SystemUserId, securityFields);
                }
            }
            return JOk(new { Views = views, Attributes = attributes, Buttons = buttons, WebResources = webResources, NoneReadFields = noneReadFields });
        }
    }
}
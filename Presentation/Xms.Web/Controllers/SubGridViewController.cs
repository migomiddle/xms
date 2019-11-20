using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Core;
using Xms.Core.Components.Platform;
using Xms.Infrastructure.Utility;
using Xms.QueryView;
using Xms.QueryView.Abstractions;
using Xms.RibbonButton;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 单据体控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class SubGridViewController : AuthenticatedControllerBase
    {
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IRibbonButtonFinder _ribbonbuttonFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IGridService _gridService;
        private readonly IFetchDataService _fetchService;
        private readonly IRoleObjectAccessService _roleObjectAccessService;

        public SubGridViewController(IWebAppContext appContext
            , IQueryViewFinder queryViewFinder
            , IRibbonButtonFinder ribbonbuttonFinder
            , IRelationShipFinder relationShipFinder
            , IGridService gridService
            , IRoleObjectAccessService roleObjectAccessService
            , IFetchDataService fetchDataService)
            : base(appContext)
        {
            _gridService = gridService;
            _fetchService = fetchDataService;
            _queryViewFinder = queryViewFinder;
            _ribbonbuttonFinder = ribbonbuttonFinder;
            _relationShipFinder = relationShipFinder;
            _roleObjectAccessService = roleObjectAccessService;
        }

        [Description("单据体记录列表")]
        [HttpPost]
        public IActionResult RenderGridView([FromBody]EntityGridModel model)
        {
            QueryView.Domain.QueryView queryView = null;
            if (model.QueryViewId.HasValue && !model.QueryViewId.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindById(model.QueryViewId.Value);
            }
            else if (model.EntityId.HasValue && !model.EntityId.Value.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityId.Value);
            }
            else if (model.EntityName.IsNotEmpty())
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityName);
            }
            else
            {
                return NotFound();
            }
            if (queryView == null)
            {
                return NotFound();
            }
            if (!queryView.IsDefault && queryView.AuthorizationEnabled)
            {
                if (!_roleObjectAccessService.Exists(queryView.QueryViewId, QueryViewDefaults.ModuleName, CurrentUser.Roles.Select(n => n.RoleId).ToArray()))
                {
                    return Unauthorized();
                }
            }
            model.QueryView = queryView;
            model.EntityId = queryView.EntityId;

            FetchDescriptor fetch = new FetchDescriptor
            {
                Page = model.Page,
                PageSize = model.PageSize,
                FetchConfig = queryView.FetchConfig,
                GetAll = !model.PagingEnabled
            };
            if (model.IsSortBySeted)
            {
                QueryColumnSortInfo sort = new QueryColumnSortInfo(model.SortBy, model.SortDirection == 0);
                fetch.Sort = sort;
            }
            //查询关键字
            if (model.Q.IsNotEmpty())
            {
                fetch.Keyword = model.Q;
                fetch.Field = model.QField;
            }
            var canFetch = true;
            //filter by relationship
            if (model.RelationShipName.IsNotEmpty())
            {
                var relationship = _relationShipFinder.FindByName(model.RelationShipName);
                if (relationship != null && model.ReferencedRecordId.HasValue && !model.ReferencedRecordId.Value.Equals(Guid.Empty))
                {
                    if (model.Filter == null)
                    {
                        model.Filter = new FilterExpression();
                    }
                    var condition = new ConditionExpression(relationship.ReferencingAttributeName, ConditionOperator.Equal, model.ReferencedRecordId);
                    model.Filter.AddCondition(condition);
                }
                model.RelationShipMeta = relationship;
            }
            if (model.RelationShipName.IsNotEmpty() && (!model.ReferencedRecordId.HasValue || model.ReferencedRecordId.Value.Equals(Guid.Empty)))
            {
                //如果被引用ID为空，则不查询数据
                canFetch = false;
            }
            fetch.Filter = model.Filter;
            if (canFetch)
            {
                fetch.User = CurrentUser;
                var datas = _fetchService.Execute(fetch);
                if (model.OnlyData)
                {
                    //var serializer = new DataContractJsonSerializer(typeof(User), new DataContractJsonSerializerSettings()
                    //{
                    //    UseSimpleDictionaryFormat = true//去掉Key和Value
                    //});
                    //return new JsonResult() { Data = datas.Items.SerializeToJson(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
                    return Content(datas.SerializeToJson());
                }
                model.Grid = _gridService.Build(queryView, _fetchService.QueryResolver.EntityList, _fetchService.QueryResolver.AttributeList);
                model.EntityList = _fetchService.QueryResolver.EntityList;
                model.AttributeList = _fetchService.QueryResolver.AttributeList;
                model.RelationShipList = _fetchService.QueryResolver.RelationShipList;
                model.Items = datas.Items;
                model.TotalItems = datas.TotalItems;
            }
            else
            {
                _fetchService.GetMetaDatas(fetch);
                model.Grid = _gridService.Build(queryView, _fetchService.QueryResolver.EntityList, _fetchService.QueryResolver.AttributeList);
                model.EntityList = _fetchService.QueryResolver.EntityList;
                model.AttributeList = _fetchService.QueryResolver.AttributeList;
                model.RelationShipList = _fetchService.QueryResolver.RelationShipList;
                model.Items = new List<dynamic>();
                model.TotalItems = 0;
            }
            var buttons = _ribbonbuttonFinder.Find(model.EntityId.Value, RibbonButtonArea.SubGrid);
            if (buttons.NotEmpty())
            {
                buttons = buttons.OrderBy(x => x.DisplayOrder).ToList();
                model.RibbonButtons = buttons;
            }
            if (model.IsSortBySeted == false)
            {
                if (_fetchService.QueryExpression.Orders.NotEmpty())
                {
                    model.SortBy = _fetchService.QueryExpression.Orders[0].AttributeName;
                    model.SortDirection = (int)_fetchService.QueryExpression.Orders[0].OrderType;
                }
            }
            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }

        [Description("一对多记录列表")]
        public IActionResult SubGridView(EntityGridModel model)
        {
            QueryView.Domain.QueryView queryView = null;
            if (model.QueryViewId.HasValue && !model.QueryViewId.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindById(model.QueryViewId.Value);
            }
            else if (model.EntityId.HasValue && !model.EntityId.Value.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityId.Value);
            }
            else if (model.EntityName.IsNotEmpty())
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityName);
            }
            else
            {
                return NotFound();
            }
            if (queryView == null)
            {
                return NotFound();
            }
            if (!queryView.IsDefault && queryView.AuthorizationEnabled)
            {
                if (!_roleObjectAccessService.Exists(queryView.QueryViewId, QueryViewDefaults.ModuleName, CurrentUser.Roles.Select(n => n.RoleId).ToArray()))
                {
                    return Unauthorized();
                }
            }
            model.QueryView = queryView;
            model.EntityId = queryView.EntityId;

            FetchDescriptor fetch = new FetchDescriptor
            {
                Page = model.Page,
                PageSize = model.PageSize,
                FetchConfig = queryView.FetchConfig,
                GetAll = !model.PagingEnabled
            };
            if (model.IsSortBySeted)
            {
                QueryColumnSortInfo sort = new QueryColumnSortInfo(model.SortBy, model.SortDirection == 0);
                fetch.Sort = sort;
            }
            //查询关键字
            if (model.Q.IsNotEmpty())
            {
                fetch.Keyword = model.Q;
                fetch.Field = model.QField;
            }
            var canFetch = true;
            //filter by relationship
            if (model.RelationShipName.IsNotEmpty())
            {
                var relationship = _relationShipFinder.FindByName(model.RelationShipName);
                if (relationship != null && model.ReferencedRecordId.HasValue && !model.ReferencedRecordId.Value.Equals(Guid.Empty))
                {
                    if (model.Filter == null)
                    {
                        model.Filter = new FilterExpression();
                    }
                    var condition = new ConditionExpression(relationship.ReferencingAttributeName, ConditionOperator.Equal, model.ReferencedRecordId);
                    model.Filter.AddCondition(condition);
                }
                model.RelationShipMeta = relationship;
            }
            if (model.RelationShipName.IsNotEmpty() && (!model.ReferencedRecordId.HasValue || model.ReferencedRecordId.Value.Equals(Guid.Empty)))
            {
                //如果被引用ID为空，则不查询数据
                canFetch = false;
            }
            fetch.Filter = model.Filter;
            if (canFetch)
            {
                fetch.User = CurrentUser;
                var datas = _fetchService.Execute(fetch);
                if (model.OnlyData)
                {
                    return Content(datas.SerializeToJson());
                }
                model.Grid = _gridService.Build(queryView, _fetchService.QueryResolver.EntityList, _fetchService.QueryResolver.AttributeList);
                model.EntityList = _fetchService.QueryResolver.EntityList;
                model.AttributeList = _fetchService.QueryResolver.AttributeList;
                model.RelationShipList = _fetchService.QueryResolver.RelationShipList;
                model.Items = datas.Items;
                model.TotalItems = datas.TotalItems;
            }
            else
            {
                _fetchService.GetMetaDatas(fetch);
                model.Grid = _gridService.Build(queryView, _fetchService.QueryResolver.EntityList, _fetchService.QueryResolver.AttributeList);
                model.EntityList = _fetchService.QueryResolver.EntityList;
                model.AttributeList = _fetchService.QueryResolver.AttributeList;
                model.RelationShipList = _fetchService.QueryResolver.RelationShipList;
                model.Items = new List<dynamic>();
                model.TotalItems = 0;
            }
            var buttons = _ribbonbuttonFinder.Query(n => n
                    .Where(w => w.StateCode == RecordState.Enabled && w.EntityId == model.EntityId.Value
                    && (w.ShowArea == RibbonButtonArea.ListHead))
                    .Sort(s => s.SortAscending(f => f.DisplayOrder)));
            model.RibbonButtons = buttons;
            if (model.IsSortBySeted == false)
            {
                if (_fetchService.QueryExpression.Orders.NotEmpty())
                {
                    model.SortBy = _fetchService.QueryExpression.Orders[0].AttributeName;
                    model.SortDirection = (int)_fetchService.QueryExpression.Orders[0].OrderType;
                }
            }
            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }
    }
}
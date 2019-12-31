using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Core;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.RibbonButton;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Abstractions;
using Xms.Schema.Entity;
using Xms.Security.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;

namespace Xms.Web.Api
{
    /// <summary>
    /// 按钮接口
    /// </summary>
    [Route("{org}/api/[controller]")]
    public class RibbonButtonController : ApiControllerBase
    {
        private readonly IRibbonButtonFinder _ribbonButtonFinder;
        private readonly IRibbonButtonUpdater _ribbonButtonUpdater;
        private readonly IEntityFinder _entityFinder;
        private readonly IDefaultButtonProvider _defaultButtonProvider;

        public RibbonButtonController(IWebAppContext appContext
            , IRibbonButtonFinder ribbonButtonFinder
            , IRibbonButtonUpdater ribbonButtonUpdater
            , IEntityFinder entityFinder
            , IDefaultButtonProvider defaultButtonProvider
            )
            : base(appContext)
        {
            _ribbonButtonFinder = ribbonButtonFinder;
            _ribbonButtonUpdater = ribbonButtonUpdater;
            _entityFinder = entityFinder;
            _defaultButtonProvider = defaultButtonProvider;
        }

        /// <summary>
        /// 查询实体按钮
        /// </summary>
        /// <param name="entityId">实体id</param>
        /// <param name="showarea">显示区域</param>
        /// <returns></returns>
        [Description("查询实体按钮")]
        [HttpGet]
        public IActionResult Get(Guid entityId, RibbonButtonArea? showarea)
        {
            FilterContainer<RibbonButton.Domain.RibbonButton> filter = FilterContainerBuilder.Build<RibbonButton.Domain.RibbonButton>();
            filter.And(x => x.EntityId == entityId);
            if (showarea.HasValue)
            {
                filter.And(x => x.ShowArea == showarea.Value);
            }
            var result = _ribbonButtonFinder.Query(x => x.Where(filter));
            return JOk(result);
        }

        /// <summary>
        /// 查询按钮
        /// </summary>
        /// <param name="id">按钮id，多个以英文逗号分隔</param>
        /// <returns></returns>
        [Description("查询按钮")]
        [HttpGet]
        public IActionResult GetById(string id)
        {
            if (id.IsNotEmpty())
            {
                var buttonId = id.SplitSafe(",").ToList().Select(x => Guid.Parse(x));
                var result = _ribbonButtonFinder.Query(x => x.Where(f => f.RibbonButtonId.In(buttonId)));
            }
            return JOk();
        }

        /// <summary>
        /// 查询所有按钮
        /// </summary>
        /// <returns></returns>
        [Description("查询所有按钮")]
        [HttpGet("getall")]
        public IActionResult GetAll()
        {
            var result = _ribbonButtonFinder.Query(x => x.Where(f => f.StateCode == RecordState.Enabled));

            return JOk(result);
        }

        /// <summary>
        /// 查询系统标准按钮
        /// </summary>
        /// <returns></returns>
        [Description("查询系统标准按钮")]
        [HttpGet("SystemButtons/{entitymask}")]
        public IActionResult GetSystemButtons(EntityMaskEnum entityMask)
        {
            return JOk(_defaultButtonProvider.Get(entityMask));
        }

        [Description("查询按钮权限资源")]
        [HttpGet("PrivilegeResource")]
        public IActionResult PrivilegeResource(bool? authorizationEnabled)
        {
            FilterContainer<RibbonButton.Domain.RibbonButton> filter = FilterContainerBuilder.Build<RibbonButton.Domain.RibbonButton>();
            filter.And(x => x.StateCode == RecordState.Enabled);
            if (authorizationEnabled.HasValue)
            {
                filter.And(x => x.AuthorizationEnabled == authorizationEnabled.Value);
            }
            var data = _ribbonButtonFinder.Query(x => x.Select(s => new { s.RibbonButtonId, s.Label, s.ShowArea, s.EntityId, s.AuthorizationEnabled }).Where(filter));
            if (data.NotEmpty())
            {
                var result = new List<PrivilegeResourceItem>();
                var entities = _entityFinder.FindAll()?.OrderBy(x => x.LocalizedName).ToList();
                foreach (var item in entities)
                {
                    var entityButtons = data.Where(x => x.EntityId == item.EntityId);
                    if (!entityButtons.Any())
                    {
                        continue;
                    }
                    var childrens = new List<PrivilegeResourceItem>();
                    var group1 = new PrivilegeResourceItem();
                    group1.GroupId = item.EntityId;
                    group1.Label = item.LocalizedName;
                    var formButtons = entityButtons.Where(x => x.ShowArea == RibbonButtonArea.Form).Select(x => (new PrivilegeResourceItem { Id = x.RibbonButtonId, Label = x.Label, AuthorizationEnabled = x.AuthorizationEnabled, GroupId = x.EntityId })).ToList();
                    if (formButtons.NotEmpty())
                    {
                        var groupForm = new PrivilegeResourceItem
                        {
                            Label = "表单",
                            Children = formButtons
                        };
                        childrens.Add(groupForm);
                    }
                    var listHeaderButtons = entityButtons.Where(x => x.ShowArea == RibbonButtonArea.ListHead).Select(x => (new PrivilegeResourceItem { Id = x.RibbonButtonId, Label = x.Label, AuthorizationEnabled = x.AuthorizationEnabled, GroupId = x.EntityId })).ToList(); ;
                    if (listHeaderButtons.NotEmpty())
                    {
                        var groupListHeader = new PrivilegeResourceItem
                        {
                            Label = "列表头部",
                            Children = listHeaderButtons
                        };
                        childrens.Add(groupListHeader);
                    }
                    var listRowButtons = entityButtons.Where(x => x.ShowArea == RibbonButtonArea.ListRow).Select(x => (new PrivilegeResourceItem { Id = x.RibbonButtonId, Label = x.Label, AuthorizationEnabled = x.AuthorizationEnabled, GroupId = x.EntityId })).ToList();
                    if (listRowButtons.NotEmpty())
                    {
                        var groupListRow = new PrivilegeResourceItem
                        {
                            Label = "列表行内",
                            Children = listRowButtons
                        };
                        childrens.Add(groupListRow);
                    }
                    var subGridButtons = entityButtons.Where(x => x.ShowArea == RibbonButtonArea.SubGrid).Select(x => (new PrivilegeResourceItem { Id = x.RibbonButtonId, Label = x.Label, AuthorizationEnabled = x.AuthorizationEnabled, GroupId = x.EntityId })).ToList();
                    if (subGridButtons.NotEmpty())
                    {
                        var groupSubGrid = new PrivilegeResourceItem
                        {
                            Label = "单据体",
                            Children = subGridButtons
                        };
                        childrens.Add(groupSubGrid);
                    }
                    group1.Children = childrens;
                    result.Add(group1);
                }
                return JOk(result);
            }
            return JOk();
        }

        [Description("启用流程权限")]
        [HttpPost("AuthorizationEnabled")]
        public IActionResult AuthorizationEnabled(UpdateAuthorizationStateModel model)
        {
            var authorizations = _ribbonButtonFinder.Query(x => x.Where(w => w.AuthorizationEnabled == true));
            if (authorizations.NotEmpty())
            {
                _ribbonButtonUpdater.UpdateAuthorization(false, authorizations.Select(x => x.RibbonButtonId).ToArray());
            }
            if (Arguments.HasValue(model.ObjectId))
            {
                _ribbonButtonUpdater.UpdateAuthorization(true, model.ObjectId);
            }
            return SaveSuccess();
        }
    }
}
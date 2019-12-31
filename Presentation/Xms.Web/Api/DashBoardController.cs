using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Form;
using Xms.Form.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Security.Abstractions;
using Xms.Solution.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;

namespace Xms.Web.Api
{
    /// <summary>
    /// 仪表板接口
    /// </summary>
    [Route("{org}/api/dashboard")]
    public class DashBoardController : ApiControllerBase
    {
        private readonly ISystemFormFinder _systemFormFinder;
        private readonly ISystemFormUpdater _systemFormUpdater;

        public DashBoardController(IWebAppContext appContext
            , ISystemFormFinder systemFormFinder
            , ISystemFormUpdater systemFormUpdater)
            : base(appContext)
        {
            _systemFormFinder = systemFormFinder;
            _systemFormUpdater = systemFormUpdater;
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _systemFormFinder.QueryPaged(x => x.Where(f => f.FormType == (int)FormType.Dashboard).Page(model.Page, model.PageSize), model.SolutionId, model.InSolution, FormType.Dashboard);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.SystemFormId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = DashBoardDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
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

        [Description("查询仪表板权限资源")]
        [HttpGet("PrivilegeResource")]
        public IActionResult PrivilegeResource(bool? authorizationEnabled)
        {
            FilterContainer<Form.Domain.SystemForm> filter = FilterContainerBuilder.Build<Form.Domain.SystemForm>();
            filter.And(x => x.StateCode == Core.RecordState.Enabled && x.FormType == (int)FormType.Dashboard);
            if (authorizationEnabled.HasValue)
            {
                filter.And(x => x.AuthorizationEnabled == authorizationEnabled.Value);
            }
            var data = _systemFormFinder.Query(x => x.Select(s => new { s.SystemFormId, s.Name, s.EntityId, s.AuthorizationEnabled })
            .Where(filter));
            if (data.NotEmpty())
            {
                return JOk(data.Select(x => (new PrivilegeResourceItem { Id = x.SystemFormId, Label = x.Name, AuthorizationEnabled = x.AuthorizationEnabled })).ToList());
            }
            return JOk();
        }

        [Description("启用表单权限")]
        [HttpPost("AuthorizationEnabled")]
        public IActionResult AuthorizationEnabled(UpdateAuthorizationStateModel model)
        {
            var authorizations = _systemFormFinder.Query(x => x.Where(w => w.AuthorizationEnabled == true && w.FormType == (int)FormType.Dashboard));
            if (authorizations.NotEmpty())
            {
                _systemFormUpdater.UpdateAuthorization(false, authorizations.Select(x => x.SystemFormId).ToArray());
            }
            if (Arguments.HasValue(model.ObjectId))
            {
                _systemFormUpdater.UpdateAuthorization(true, model.ObjectId);
            }
            return SaveSuccess();
        }
    }
}
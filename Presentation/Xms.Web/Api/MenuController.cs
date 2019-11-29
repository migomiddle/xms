using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.SiteMap;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;

namespace Xms.Web.Api
{
    /// <summary>
    /// 菜单接口
    /// </summary>
    [Route("{org}/api/[controller]")]
    public class MenuController : ApiControllerBase
    {
        private readonly IPrivilegeService _privilegeService;
        private readonly IPrivilegeTreeBuilder _privilegeTreeBuilder;

        public MenuController(IWebAppContext appContext
            , IPrivilegeService privilegeService
            , IPrivilegeTreeBuilder privilegeTreeBuilder)
            : base(appContext)
        {
            _privilegeService = privilegeService;
            _privilegeTreeBuilder = privilegeTreeBuilder;
        }

        [Description("查询菜单权限资源")]
        [HttpGet("PrivilegeResource")]
        public IActionResult PrivilegeResource()
        {
            var result = _privilegeTreeBuilder.Build(x => x
            .Where(f => f.OrganizationId == CurrentUser.OrganizationId)
            .Sort(s => s.SortAscending(ss => ss.DisplayOrder))
            );
            return JOk(result);
        }

        [Description("启用菜单权限")]
        [HttpPost("AuthorizationEnabled")]
        public IActionResult AuthorizationEnabled(UpdateAuthorizationStateModel model)
        {
            var authorizations = _privilegeService.Query(x => x.Where(w => w.AuthorizationEnabled == true));
            if (authorizations.NotEmpty())
            {
                _privilegeService.UpdateAuthorization(false, authorizations.Select(x => x.PrivilegeId).ToArray());
            }
            if (Arguments.HasValue(model.ObjectId))
            {
                _privilegeService.UpdateAuthorization(true, model.ObjectId);
            }
            return SaveSuccess();
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.SiteMap;
using Xms.SiteMap.Domain;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 菜单控制器
    /// </summary>
    public class PrivilegeController : AuthorizedControllerBase
    {
        private readonly IPrivilegeService _privilegeService;
        private readonly IPrivilegeTreeBuilder _privilegeTreeBuilder;

        public PrivilegeController(IWebAppContext appContext
            , IPrivilegeService privilegeService
            , IPrivilegeTreeBuilder privilegeTreeBuilder)
            : base(appContext)
        {
            _privilegeService = privilegeService;
            _privilegeTreeBuilder = privilegeTreeBuilder;
        }

        [Description("权限项")]
        public IActionResult Index(PrivilegeModel model, bool isAutoComplete = false)
        {
            if (IsRequestJson)
            {
                if (!isAutoComplete)
                {
                    FilterContainer<Privilege> filter = FilterContainerBuilder.Build<Privilege>();
                    filter.And(n => n.OrganizationId == CurrentUser.OrganizationId);
                    if (model.IsEnable.HasValue)
                    {
                        filter.And(n => n.AuthorizationEnabled == model.IsEnable.Value);
                    }
                    if (model.IsShowAsMenu.HasValue)
                    {
                        filter.And(n => n.IsVisibled == model.IsShowAsMenu.Value);
                    }
                    var result = _privilegeTreeBuilder.Build(x => x
                    .Where(filter)
                    .Sort(s => s.SortAscending(ss => ss.DisplayOrder))
                    );
                    return JOk(result);
                }
                else
                {
                    FilterContainer<Privilege> filter = FilterContainerBuilder.Build<Privilege>();
                    filter.And(n => n.OrganizationId == CurrentUser.OrganizationId);
                    if (model.MethodName.IsNotEmpty())
                    {
                        filter.And(n => n.MethodName.Like(model.MethodName));
                    }
                    if (model.DisplayName.IsNotEmpty())
                    {
                        filter.And(n => n.DisplayName.Like(model.DisplayName));
                    }
                    if (model.GetAll)
                    {
                        var result = _privilegeService.Query(x => x
                            .Where(filter)
                            .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                            );
                        model.Items = result;
                        model.TotalItems = result.Count;
                    }
                    else
                    {
                        PagedList<Privilege> result = _privilegeService.QueryPaged(x => x
                            .Page(model.Page, model.PageSize)
                            .Where(filter)
                            .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                            );

                        model.Items = result.Items;
                        model.TotalItems = result.TotalItems;
                    }
                    return JOk(model);
                }
            }
            return View("~/Views/Security/Privileges.cshtml");
        }

        [HttpGet]
        [Description("权限项编辑")]
        public IActionResult EditPrivilege(Guid? id)
        {
            EditPrivilegeModel model = new EditPrivilegeModel();
            if (id.HasValue && !id.Equals(Guid.Empty))
            {
                Privilege entity = _privilegeService.FindById(id.Value);
                if (IsRequestJson)
                {
                    return JOk(entity);
                }
            }

            return JOk(null);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("权限项编辑")]
        public IActionResult EditPrivilege(EditPrivilegeModel model)
        {
            if (ModelState.IsValid)
            {
                Privilege entity = (model.PrivilegeId.HasValue && !model.PrivilegeId.Value.Equals(Guid.Empty)) ? _privilegeService.FindById(model.PrivilegeId.Value) : new Privilege();

                model.CopyTo(entity);

                if (model.PrivilegeId.HasValue && !model.PrivilegeId.Value.Equals(Guid.Empty))
                {
                    return _privilegeService.Update(entity).UpdateResult(T);
                }
                else
                {
                    entity.PrivilegeId = Guid.NewGuid();
                    entity.OrganizationId = CurrentUser.OrganizationId;
                    return _privilegeService.Create(entity).CreateResult(T);
                }
            }
            return JError(GetModelErrors());
        }

        [Description("权限项移动排序")]
        public IActionResult MovePrivilege(Guid moveid, Guid targetid, Guid parentid, string position)
        {
            int status = _privilegeService.Move(moveid, targetid, parentid, position);

            if (status == 1)
            {
                return SaveSuccess();
            }
            else
            {
                return SaveFailure();
            }
        }

        [Description("删除权限项")]
        [HttpPost]
        public IActionResult DeletePrivilege([FromBody]DeleteManyModel model)
        {
            return _privilegeService.DeleteById(model.RecordId).DeleteResult(T);
        }
    }
}
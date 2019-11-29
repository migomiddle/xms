using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Event.Abstractions;
using Xms.Form.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.QueryView.Abstractions;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.Abstractions;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Customize.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Customize.Controllers
{
    /// <summary>
    /// 实体管理控制器
    /// </summary>
    public class EntityController : CustomizeBaseController
    {
        private readonly IEntityFinder _entityFinder;

        public EntityController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityFinder entityFinder
            )
            : base(appContext, solutionService)
        {
            _entityFinder = entityFinder;
        }

        [Description("实体列表")]
        public IActionResult Index(EntityModel model)
        {
            if (!model.LoadData)
            {
                return DynamicResult(model);
            }
            FilterContainer<Schema.Domain.Entity> filter = FilterContainerBuilder.Build<Schema.Domain.Entity>();
            filter.And(n => n.OrganizationId == CurrentUser.OrganizationId);
            if (model.Name.IsNotEmpty())
            {
                filter.And(n => n.Name == model.Name);
            }
            if (!model.IsSortBySeted)
            {
                model.SortBy = "Name";
                model.SortDirection = (int)SortDirection.Asc;
            }
            if (model.GetAll)
            {
                model.Page = 1;
                model.PageSize = WebContext.PlatformSettings.MaxFetchRecords;
            }
            else if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            if (!model.EntityGroupId.IsEmpty())
            {
                filter.And(n => n.EntityGroups.Like(model.EntityGroupId.ToString()));
            }
            model.PageSize = model.PageSize > WebContext.PlatformSettings.MaxFetchRecords ? WebContext.PlatformSettings.MaxFetchRecords : model.PageSize;
            PagedList<Schema.Domain.Entity> result = _entityFinder.QueryPaged(x => x
                .Page(model.Page, model.PageSize)
                .Where(filter)
                .Sort(n => n.OnFile(model.SortBy).ByDirection(model.SortDirection))
                , SolutionId.Value, true);

            model.Items = result.Items;
            model.TotalItems = result.TotalItems;
            model.SolutionId = SolutionId.Value;

            return DynamicResult(model);
        }

        [Description("检查实体是否已存在")]
        public IActionResult Exists(string name)
        {
            if (_entityFinder.Exists(name))
            {
                return JError(T["name_already_exists"]);
            }
            return JOk("");
        }
    }

    [Route("{org}/[area]/entity/[action]")]
    public class CreateEntityController : CustomizeBaseController
    {
        private readonly IEntityCreater _entityCreater;
        private readonly IEventPublisher _eventPublisher;

        public CreateEntityController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityCreater entityCreater
            , IEventPublisher eventPublisher
            )
            : base(appContext, solutionService)
        {
            _entityCreater = entityCreater;
            _eventPublisher = eventPublisher;
        }

        [HttpGet]
        [Description("新建实体")]
        public IActionResult CreateEntity()
        {
            CreateEntityModel model = new CreateEntityModel
            {
                EntityMask = EntityMaskEnum.User,
                SolutionId = SolutionId.Value
            };
            return View($"~/Areas/{WebContext.Area}/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("新建实体-保存")]
        public IActionResult CreateEntity(CreateEntityModel model)
        {
            if (ModelState.IsValid)
            {
                model.Name = model.Name.Trim();
                var entity = new Schema.Domain.Entity();
                model.CopyTo(entity);
                entity.EntityId = Guid.NewGuid();
                entity.IsCustomizable = true;
                entity.CreatedBy = CurrentUser.SystemUserId;
                entity.OrganizationId = CurrentUser.OrganizationId;
                if (model.EntityGroupId.NotEmpty())
                {
                    entity.EntityGroups = model.EntityGroupId.SerializeToJson();
                }
                if (_entityCreater.Create(entity, model.DefaultAttributes))
                {
                    //创建默认按钮
                    if (model.DefaultButtons.NotEmpty())
                    {
                        _eventPublisher.Publish(new CreateDefaultButtonsEvent(entity, model.DefaultButtons));
                    }
                    //创建默认表单
                    if (model.CreateDefaultForm)
                    {
                        _eventPublisher.Publish(new CreateDefaultFormEvent(entity));
                    }
                    //创建默认视图
                    if (model.CreateDefaultView)
                    {
                        _eventPublisher.Publish(new CreateDefaultViewEvent(entity));
                    }
                    return CreateSuccess(new { id = entity.EntityId });
                }
                return CreateFailure();
            }
            return CreateFailure(GetModelErrors());
        }
    }

    [Route("{org}/[area]/entity/[action]")]
    public class UpdateEntityController : CustomizeBaseController
    {
        private readonly IEntityUpdater _entityUpdater;
        private readonly IEntityFinder _entityFinder;

        public UpdateEntityController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityUpdater entityUpdater
            , IEntityFinder entityFinder
            )
            : base(appContext, solutionService)
        {
            _entityUpdater = entityUpdater;
            _entityFinder = entityFinder;
        }

        [HttpGet]
        [Description("实体编辑")]
        public IActionResult EditEntity(Guid id)
        {
            EditEntityModel model = new EditEntityModel();
            if (!id.IsEmpty())
            {
                var entity = _entityFinder.FindById(id);
                if (entity != null)
                {
                    entity.CopyTo(model);
                    model.Entity = entity;
                    model.ParentEntityLocalizedName = entity.ParentEntityId.HasValue ? _entityFinder.FindById(entity.ParentEntityId.Value)?.LocalizedName : string.Empty;
                    if (entity.EntityGroups.IsNotEmpty())
                    {
                        model.EntityGroupId = new Guid[] { }.DeserializeFromJson(entity.EntityGroups);
                    }
                    return View($"~/Areas/{WebContext.Area}/Views/Entity/{WebContext.ActionName}.cshtml", model);
                }
            }
            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Description("实体信息保存")]
        public IActionResult EditEntity(EditEntityModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _entityFinder.FindById(model.EntityId);
                model.IsCustomizable = entity.IsCustomizable;
                model.CopyTo(entity);
                if (model.EntityGroupId.NotEmpty())
                {
                    entity.EntityGroups = model.EntityGroupId.SerializeToJson();
                }
                _entityUpdater.Update(entity);
                return UpdateSuccess(new { id = entity.EntityId });
            }
            return UpdateFailure(GetModelErrors());
        }
    }

    [Route("{org}/[area]/entity/[action]")]
    public class DeleteEntityController : CustomizeBaseController
    {
        private readonly IEntityDeleter _entityDeleter;

        public DeleteEntityController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityDeleter entityDeleter
            )
            : base(appContext, solutionService)
        {
            _entityDeleter = entityDeleter;
        }

        [Description("删除实体")]
        [HttpPost]
        public IActionResult DeleteEntity([FromBody]DeleteEntityModel model)
        {
            return _entityDeleter.DeleteById(model.DeleteTable, model.RecordId).DeleteResult(T);
        }
    }
}
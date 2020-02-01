using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Api.Core.Controller;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Api.Models;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;

namespace Xms.Schema.Api.Controllers
{
    
    [Route("{org}/api/schema/entity/create")]
    public class EntityCreaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityCreater _entityCreater;
        private readonly IEntityFinder _entityFinder;
        public EntityCreaterController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityCreater entityCreater
            , IEntityFinder entityFinder
            )
            : base(appContext, solutionService)
        {
            _entityCreater = entityCreater;
            _entityFinder = entityFinder;
        }
                
        [Description("新建实体-保存")]        
        [HttpPost]
        public IActionResult Post(CreateEntityModel model)
        {
            if (ModelState.IsValid)
            {
                if (_entityFinder.FindByName(model.Name) != null)
                {
                    return JError(T["name_already_exists"]);
                }
                model.Name = model.Name.Trim();
                var entity = new Schema.Domain.Entity();
                model.CopyTo(entity);
                entity.SolutionId = _solutionId.Value;
                entity.IsCustomizable = true;
                entity.EntityId = Guid.NewGuid();
                entity.CreatedBy = CurrentUser.SystemUserId;
                entity.CreatedOn = DateTime.Now;
                entity.OrganizationId = CurrentUser.OrganizationId;
                _entityCreater.Create(entity);

                return CreateSuccess(new { id = entity.EntityId });
            }
            return CreateFailure(GetModelErrors());
        }
        
    }
}

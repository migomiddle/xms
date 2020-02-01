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
    
    [Route("{org}/api/schema/entity/update")]
    public class EntityUpdaterController : ApiCustomizeControllerBase
    {
        private readonly IEntityUpdater _entityUpdater;
        private readonly IEntityFinder _entityFinder;
        public EntityUpdaterController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityUpdater entityCreater
            , IEntityFinder entityFinder
            )
            : base(appContext, solutionService)
        {
            _entityUpdater = entityCreater;
            _entityFinder = entityFinder;
        }
               

        [HttpPost]        
        [Description("实体信息保存")]
        public IActionResult Post(EditEntityModel model)
        {
            if (ModelState.IsValid)
            {
                var entity = _entityFinder.FindById(model.EntityId);
                if (entity == null)
                {
                    return NotFound();
                }
                model.IsCustomizable = entity.IsCustomizable;
                model.CopyTo(entity);
                _entityUpdater.Update(entity);
                return UpdateSuccess(new { id = entity.EntityId });
            }
            return UpdateFailure(GetModelErrors());
        }

    }
}

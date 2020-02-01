using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Xms.Api.Core.Controller;
using Xms.Schema.Entity;
using Xms.Solution;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Schema.Api.Controllers
{

    [Route("{org}/api/schema/entity/delete")]
    public class EntityDeleterController : ApiCustomizeControllerBase
    {
        private readonly IEntityDeleter _entityDeleter;
        public EntityDeleterController(IWebAppContext appContext
            , ISolutionService solutionService
            , IEntityDeleter entityDeleter
            )
            : base(appContext, solutionService)
        {
            _entityDeleter = entityDeleter;
        }

        [Description("删除实体")]
        [HttpPost]
        public IActionResult Delete(DeleteManyModel model)
        {
            return _entityDeleter.DeleteById(id:model.RecordId).DeleteResult(T);
        }

    }
}

using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Xms.Infrastructure.Utility;
using Xms.Schema.Domain;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.EntityData.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Mvc;

namespace Xms.EntityData.Api
{
    /// <summary>
    /// 数据删除接口
    /// </summary>
    [Route("{org}/api/data/delete")]
    public class DataDeleterController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IDataDeleter _dataDeleter;

        public DataDeleterController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IDataDeleter dataDeleter)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _dataDeleter = dataDeleter;
        }

        [Description("删除记录")]
        [HttpPost]
        public IActionResult Delete(DeleteEntityRecordModel model)
        {
            if (model.RecordId.IsEmpty())
            {
                return NotSpecifiedRecord();
            }
            Entity entityMetadata = null;
            if (model.EntityName.IsNotEmpty())
            {
                entityMetadata = _entityFinder.FindByName(model.EntityName);
            }
            else
            {
                entityMetadata = _entityFinder.FindById(model.EntityId);
            }
            if(entityMetadata == null)
            {
                return NotFound();
            }
            return _dataDeleter.Delete(entityMetadata.Name, model.RecordId).DeleteResult(T);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 实体数据合并控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class EntityMergeController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataFinder _dataFinder;

        public EntityMergeController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IDataFinder dataFinder
            )
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _dataFinder = dataFinder;
        }

        #region 合并

        [Description("合并记录")]
        public IActionResult Merge(Guid entityid, Guid recordid1, Guid recordid2)
        {
            MergeModel model = new MergeModel();
            if (entityid.Equals(Guid.Empty) || recordid1.Equals(Guid.Empty) || recordid2.Equals(Guid.Empty))
            {
                return JError(T["parameter_error"]);
            }
            model.EntityMetas = _entityFinder.FindById(entityid);
            model.Attributes = _attributeFinder.FindByEntityId(entityid);
            model.Record1 = _dataFinder.RetrieveById(model.EntityMetas.Name, recordid1);
            model.Record2 = _dataFinder.RetrieveById(model.EntityMetas.Name, recordid2);
            model.EntityId = entityid;
            model.RecordId1 = recordid1;
            model.RecordId2 = recordid2;
            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }

        #endregion 合并
    }
}
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Infrastructure.Utility;
using Xms.Schema.RelationShip;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Schema.Api
{
    /// <summary>
    /// 关系接口
    /// </summary>
    [Route("{org}/api/schema/relationship")]
    public class RelationShipController : ApiControllerBase
    {
        private readonly IRelationShipFinder _relationShipFinder;

        public RelationShipController(IWebAppContext appContext
            , IRelationShipFinder relationShipService)
            : base(appContext)
        {
            _relationShipFinder = relationShipService;
        }

        [Description("查询关系元数据")]
        [HttpGet("{name}")]
        public IActionResult Get(string name)
        {
            if (name.IsEmpty())
            {
                return NotFound();
            }
            var result = _relationShipFinder.FindByName(name);
            return JOk(result.SerializeToJson());
        }

        [Description("实体引用关系列表")]
        [HttpGet("GetReferencing/{referencingEntityId}")]
        public IActionResult GetReferencing(Guid referencingEntityId)
        {
            List<Schema.Domain.RelationShip> result = _relationShipFinder.QueryByEntityId(referencingEntityId, null);

            return JOk(result);
        }

        [Description("实体被引用关系列表")]
        [HttpGet("GetReferenced/{referencedEntityId}")]
        public IActionResult GetReferenced(Guid referencedEntityId)
        {
            List<Schema.Domain.RelationShip> result = _relationShipFinder.QueryByEntityId(null, referencedEntityId);

            return JOk(result);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.Schema.Abstractions;
using Xms.Schema.Api.Models;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Schema.Api
{
    /// <summary>
    /// 字段元数据接口删除
    /// </summary>
    [Route("{org}/api/schema/attribute/delete")]
    public class AttributeDeleterController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IAttributeDeleter _attributeDeleter;

        public AttributeDeleterController(IWebAppContext appContext
            , IEntityFinder entityService
            , IAttributeFinder attributeService
            ,IAttributeDeleter attributeDeleter)
            : base(appContext)
        {
            _entityFinder = entityService;
            _attributeFinder = attributeService;
            _attributeDeleter = attributeDeleter;
        }


        [Description("删除字段")]
        [HttpPost]
        public IActionResult DeleteAttribute([FromBody]DeleteManyModel model)
        {
            return _attributeDeleter.DeleteById(model.RecordId).DeleteResult(T);
        }
    }
}
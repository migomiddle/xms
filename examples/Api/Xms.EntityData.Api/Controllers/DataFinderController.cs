using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using Xms.EntityData.Api.Models;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.EntityData.Api
{
    /// <summary>
    /// 数据查询接口
    /// </summary>
    [Route("{org}/api/data/retrieve")]
    public class DataFinderController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IDataFinder _dataFinder;

        public DataFinderController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _dataFinder = dataFinder;
        }

        [Description("查找引用记录")]
        [HttpGet("ReferencedRecord/{entityid}/{value}/{allcolumns?}")]
        public IActionResult RetrieveReferencedRecord(RetrieveReferencedRecordModel args)
        {
            if (args.EntityId.Equals(Guid.Empty) || args.Value.IsEmpty())
            {
                return NotFound();
            }
            var entity = _entityFinder.FindById(args.EntityId);
            if (entity == null)
            {
                return NotFound();
            }
            var entityName = entity.Name;
            QueryByAttribute qba = new QueryByAttribute(entityName, CurrentUser.UserSettings.LanguageId);
            if (args.AllColumns)
            {
                qba.ColumnSet.AllColumns = true;
            }
            else
            {
                qba.ColumnSet.AddColumns(entityName + "id", "name");
            }
            qba.Attributes.Add(entityName + "id");
            qba.Values.Add(args.Value);

            var result = _dataFinder.Retrieve(qba);
            if (result != null && result.Count > 0)
            {
                if (!args.AllColumns)
                {
                    result.AddIfNotContain("id", result[entityName + "id"]);
                    result.Remove(entityName + "id");
                }
                else
                {
                    result = DataHelper.WrapOptionName(_attributeFinder.FindByEntityId(args.EntityId), result);
                }
            }

            return JOk(result);
        }

        [Description("查找一条记录")]
        [HttpPost("single")]
        public IActionResult Retrieve(QueryExpression query)
        {
            if (query.EntityName.IsEmpty())
            {
                return JError("entityname is not specified");
            }
            var entity = _entityFinder.FindByName(query.EntityName);
            if (entity == null)
            {
                return JError("entityname is not found");
            }

            var result = _dataFinder.Retrieve(query);

            return JOk(result);
        }

        [Description("查找一条记录")]
        [HttpGet("{entityname}/{id}")]
        public IActionResult RetrieveById([FromRoute]RetrieveByIdModel args)
        {
            if (args.EntityName.IsEmpty())
            {
                return JError("entityname is not specified");
            }
            var entity = _entityFinder.FindByName(args.EntityName);
            if (entity == null)
            {
                return JError("entityname is not found");
            }

            var result = _dataFinder.RetrieveById(args.EntityName, args.Id);

            return JOk(result);
        }

        [Description("查找多条记录")]
        [HttpPost("Multiple")]
        public IActionResult RetrieveMultiple(RetrieveMultipleModel args)
        {
            if (args.Query.EntityName.IsEmpty())
            {
                return JError("entityname is not specified");
            }
            var entity = _entityFinder.FindByName(args.Query.EntityName);
            if (entity == null)
            {
                return JError("entityname is not found");
            }
            if (args.IsAll)
            {
                var result = _dataFinder.RetrieveAll(args.Query);
                return JOk(result);
            }
            else
            {
                var result = _dataFinder.RetrieveMultiple(args.Query);
                return JOk(result);
            }
        }
    }
}
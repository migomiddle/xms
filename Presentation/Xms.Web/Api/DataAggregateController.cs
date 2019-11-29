using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.QueryView;
using Xms.Schema.Attribute;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    /// <summary>
    /// 实体数据统计接口
    /// </summary>
    [Route("{org}/api/data/aggregate")]
    public class DataAggregateController : ApiControllerBase
    {
        private readonly IAggregateService _aggregateService;
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IAttributeFinder _attributeFinder;

        public DataAggregateController(IWebAppContext appContext
            , IAggregateService aggregateService
            , IQueryViewFinder queryViewFinder
            , IAttributeFinder attributeFinder)
            : base(appContext)
        {
            _aggregateService = aggregateService;
            _queryViewFinder = queryViewFinder;
            _attributeFinder = attributeFinder;
        }

        /// <summary>
        /// 获取统计数据
        /// </summary>
        /// <param name="queryViewId"></param>
        /// <returns></returns>
        [Description("获取统计数据")]
        [HttpPost]
        public IActionResult Post(AggregateModel model)
        {
            var queryView = _queryViewFinder.FindById(model.QueryViewId);
            if (queryView == null)
            {
                return NotFound();
            }
            if (queryView != null && queryView.AggregateConfig.IsNotEmpty())
            {
                var aggFields = new List<AggregateExpressionField>().DeserializeFromJson(queryView.AggregateConfig);
                if (aggFields.NotEmpty())
                {
                    var queryExp = new QueryExpression().DeserializeFromJson(queryView.FetchConfig);
                    if (model.Filter != null
                        && (model.Filter.Conditions.NotEmpty() || (model.Filter.Filters.NotEmpty() && model.Filter.Filters.First().Conditions.NotEmpty())))
                    {
                        queryExp.Criteria.AddFilter(model.Filter);
                    }
                    var aggExp = new AggregateExpression
                    {
                        ColumnSet = queryExp.ColumnSet,
                        Criteria = queryExp.Criteria,
                        EntityName = queryExp.EntityName,
                        LinkEntities = queryExp.LinkEntities,
                        AggregateFields = aggFields
                    };
                    var aggDatas = _aggregateService.Execute(aggExp);
                    var attributes = _attributeFinder.FindByName(queryView.EntityId, aggFields.Select(x => x.AttributeName).ToArray());
                    foreach (dynamic item in aggDatas)
                    {
                        var line = item as IDictionary<string, object>;
                        var attribute = attributes.Find(x => x.Name.IsCaseInsensitiveEqual(line.Keys.First()));
                        item.metadata = new { attribute.Name, attribute.LocalizedName, attribute.AttributeTypeName, attribute.EntityId, attribute.EntityName, attribute.EntityLocalizedName };
                        item.aggregatetype = aggFields.Find(x => x.AttributeName.IsCaseInsensitiveEqual(attribute.Name)).AggregateType;
                    }
                    return JOk(new { View = new { queryView.QueryViewId, queryView.Name }, Data = aggDatas });
                }
            }
            return JOk();
        }
    }
}
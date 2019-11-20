using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.QueryView;
using Xms.QueryView.Abstractions;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.Extensions;
using Xms.Schema.OptionSet;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 看板视图控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class KanbanViewController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IOptionSetFinder _optionSetFinder;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly IAggregateService _aggregateService;

        public KanbanViewController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IQueryViewFinder queryViewFinder
            , IOptionSetFinder optionSetFinder
            , IRoleObjectAccessService roleObjectAccessService
            , IAggregateService aggregateService)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _queryViewFinder = queryViewFinder;
            _optionSetFinder = optionSetFinder;
            _roleObjectAccessService = roleObjectAccessService;
            _aggregateService = aggregateService;
        }

        [Description("记录列表")]
        public IActionResult KanbanView([FromBody, FromQuery]KanbanGridModel model)
        {
            if (model.AggregateField.IsEmpty() || model.GroupField.IsEmpty())
            {
                return JError("请指定统计字段及分组字段");
            }
            QueryView.Domain.QueryView queryView = null;
            if (model.QueryId.HasValue && !model.QueryId.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindById(model.QueryId.Value);
            }
            if (queryView == null)
            {
                return NotFound();
            }
            if (!queryView.IsDefault && queryView.AuthorizationEnabled)
            {
                if (!_roleObjectAccessService.Exists(queryView.QueryViewId, QueryViewDefaults.ModuleName, CurrentUser.Roles.Select(n => n.RoleId).ToArray()))
                {
                    return Unauthorized();
                }
            }
            model.QueryView = queryView;
            model.EntityId = queryView.EntityId;
            model.QueryId = queryView.QueryViewId;
            model.EntityName = _entityFinder.FindById(model.EntityId.Value).Name;
            var attributes = new List<Schema.Domain.Attribute>();
            var aggAttr = _attributeFinder.Find(model.EntityId.Value, model.AggregateField);
            attributes.Add(aggAttr);
            var queryExp = new QueryExpression();
            queryExp = queryExp.DeserializeFromJson(queryView.FetchConfig);
            var orderExp = new OrderExpression("createdon", OrderType.Descending);
            queryExp.Orders.Add(orderExp);
            Dictionary<string, AggregateType> attributeAggs = new Dictionary<string, AggregateType>();
            if (aggAttr.TypeIsInt() || aggAttr.TypeIsFloat() || aggAttr.TypeIsDecimal() || aggAttr.TypeIsMoney())
            {
                attributeAggs.Add(model.AggregateField, AggregateType.Sum);
                model.AggType = AggregateType.Sum;
            }
            else
            {
                attributeAggs.Add(model.AggregateField, AggregateType.Count);
                model.AggType = AggregateType.Count;
            }
            var datas = _aggregateService.Execute(queryExp, attributeAggs, new List<string>() { model.GroupField });
            model.Items = datas;
            queryExp.ColumnSet.Columns.Clear();
            queryExp.ColumnSet.AddColumns("createdon", model.AggregateField, model.GroupField, "ownerid");
            model.GroupingDatas = _aggregateService.GroupingTop(model.GroupingTop, model.GroupField, queryExp, orderExp);
            var groupAttr = _attributeFinder.Find(model.EntityId.Value, model.GroupField);
            groupAttr.OptionSet = _optionSetFinder.FindById(groupAttr.OptionSetId.Value);
            attributes.Add(groupAttr);
            model.AttributeList = attributes;
            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }
    }
}
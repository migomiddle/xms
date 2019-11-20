using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Core;
using Xms.Core.Components.Platform;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Infrastructure.Utility;
using Xms.QueryView;
using Xms.QueryView.Abstractions;
using Xms.RibbonButton;
using Xms.RibbonButton.Abstractions;
using Xms.Schema.RelationShip;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Client;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 数据列表控制器
    /// </summary>
    [Route("{org}/entity/[action]")]
    public class GridViewController : AuthenticatedControllerBase
    {
        private readonly IQueryViewFinder _queryViewFinder;
        private readonly IRibbonButtonFinder _ribbonbuttonFinder;
        private readonly IGridService _gridService;
        private readonly IFetchDataService _fetchDataService;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly IAggregateService _aggregateService;
        private readonly IRelationShipFinder _relationShipFinder;

        public GridViewController(IWebAppContext appContext
            , IQueryViewFinder queryViewFinder
            , IRibbonButtonFinder ribbonbuttonFinder
            , IGridService gridService
            , IRoleObjectAccessService roleObjectAccessService
            , IFetchDataService fetchDataService
            , IAggregateService aggregateService
            , IRelationShipFinder relationShipFinder)
            : base(appContext)
        {
            _gridService = gridService;
            _fetchDataService = fetchDataService;
            _queryViewFinder = queryViewFinder;
            _ribbonbuttonFinder = ribbonbuttonFinder;
            _roleObjectAccessService = roleObjectAccessService;
            _aggregateService = aggregateService;
            _relationShipFinder = relationShipFinder;
        }

        [Description("记录列表")]
        [Route("{entityname?}")]
        public IActionResult List(DataListModel model)
        {
            if (!model.EntityId.HasValue || model.EntityId.Equals(Guid.Empty))
            {
                if (!model.QueryViewId.HasValue || model.QueryViewId.Equals(Guid.Empty))
                {
                    if (model.EntityName.IsEmpty())
                    {
                        return NotFound();
                    }
                }
            }

            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }

        [Description("记录列表")]
        [HttpGet]
        public IActionResult GridView(DataListModel args)
        {
            EntityGridModel model = new EntityGridModel();
            args.CopyTo(model);
            return GridView(model);
        }

        [Description("记录列表")]
        [HttpPost]
        public IActionResult GridView([FromBody]EntityGridModel model)
        {
            QueryView.Domain.QueryView queryView = null;
            if (model.QueryViewId.HasValue && !model.QueryViewId.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindById(model.QueryViewId.Value);
            }
            else if (model.EntityId.HasValue && !model.EntityId.Value.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityId.Value);
            }
            else if (model.EntityName.IsNotEmpty())
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityName);
            }
            else
            {
                return NotFound();
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
            //EntityGridModel model = new EntityGridModel();
            model.QueryView = queryView;
            model.EntityId = queryView.EntityId;
            model.QueryViewId = queryView.QueryViewId;
            model.TargetFormId = queryView.TargetFormId;
            if (model.IsEnabledViewSelector)
            {
                model.QueryViews = _queryViewFinder.QueryAuthorized(n => n.Where(f => f.EntityId == model.EntityId && f.StateCode == RecordState.Enabled)
                .Sort(s => s.SortAscending(f => f.Name)));
            }
            if (!model.PageSizeBySeted && CurrentUser.UserSettings.PagingLimit > 0)
            {
                model.PageSize = CurrentUser.UserSettings.PagingLimit;
            }
            FetchDescriptor fetch = new FetchDescriptor
            {
                User = CurrentUser,
                Page = model.Page,
                PageSize = model.PageSize,
                FetchConfig = queryView.FetchConfig
            };
            if (model.IsSortBySeted)
            {
                QueryColumnSortInfo sort = new QueryColumnSortInfo(model.SortBy, model.SortDirection == 0);
                fetch.Sort = sort;
            }
            fetch.Filter = model.Filter;

            if (model.Q.IsNotEmpty())
            {
                fetch.Keyword = model.Q;
                fetch.Field = model.QField;
            }
            //buttons
            if (model.IsShowButtons)
            {
                FilterContainer<RibbonButton.Domain.RibbonButton> buttonFilter = FilterContainerBuilder.Build<RibbonButton.Domain.RibbonButton>()
                    .And(w => w.StateCode == RecordState.Enabled && w.EntityId == model.EntityId.Value
                        && (w.ShowArea == RibbonButtonArea.ListHead || w.ShowArea == RibbonButtonArea.ListRow));
                if (model.QueryView.IsCustomButton && model.QueryView.CustomButtons.IsNotEmpty())
                {
                    List<Guid> buttonid = new List<Guid>();
                    buttonid = buttonid.DeserializeFromJson(model.QueryView.CustomButtons);
                    buttonFilter.And(w => w.RibbonButtonId.In(buttonid));
                }
                var buttons = _ribbonbuttonFinder.Query(n => n
                .Where(buttonFilter)
                .Sort(s => s.SortAscending(f => f.DisplayOrder)));
                model.RibbonButtons = buttons;
            }

            //var datas = _fetchService.Execute(fetch);
            _fetchDataService.GetMetaDatas(fetch);
            model.Grid = _gridService.Build(queryView, _fetchDataService.QueryResolver.EntityList, _fetchDataService.QueryResolver.AttributeList);
            model.EntityList = _fetchDataService.QueryResolver.EntityList;
            model.AttributeList = _fetchDataService.QueryResolver.AttributeList;
            model.RelationShipList = _fetchDataService.QueryResolver.RelationShipList;
            //model.Items = datas.Items;
            //model.TotalItems = datas.TotalItems;
            //model.TotalPages = datas.TotalPages;
            //aggregation
            if (queryView.AggregateConfig.IsNotEmpty())
            {
                var aggFields = new List<AggregateExpressionField>().DeserializeFromJson(queryView.AggregateConfig);
                if (aggFields.NotEmpty())
                {
                    var aggExp = new AggregateExpression
                    {
                        ColumnSet = _fetchDataService.QueryExpression.ColumnSet,
                        Criteria = _fetchDataService.QueryExpression.Criteria,
                        EntityName = _fetchDataService.QueryExpression.EntityName,
                        LinkEntities = _fetchDataService.QueryExpression.LinkEntities,
                        AggregateFields = aggFields
                    };
                    var aggDatas = _aggregateService.Execute(aggExp);
                    model.AggregationData = aggDatas.NotEmpty() ? aggDatas.First() : null;
                    model.AggregateFields = aggFields;
                }
            }
            if (model.IsSortBySeted == false)
            {
                if (_fetchDataService.QueryExpression.Orders.NotEmpty())
                {
                    model.SortBy = _fetchDataService.QueryExpression.Orders[0].AttributeName;
                    model.SortDirection = (int)_fetchDataService.QueryExpression.Orders[0].OrderType;
                }
            }
            //字段权限
            model.NonePermissionFields = _fetchDataService.NonePermissionFields;

            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }

        [Description("选择实体数据对话框")]
        public IActionResult RecordsDialog([FromBody]SelectEntityRecordsDialogModel model, DialogModel dm)
        {
            QueryView.Domain.QueryView queryView = null;
            if (model.QueryId.HasValue && !model.QueryId.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindById(model.QueryId.Value);
            }
            else if (model.EntityId.HasValue && !model.EntityId.Equals(Guid.Empty))
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityId.Value);
            }
            else if (model.EntityName.IsNotEmpty())
            {
                queryView = _queryViewFinder.FindEntityDefaultView(model.EntityName);
            }
            else
            {
                return NotFound();
            }
            if (queryView == null)
            {
                return NotFound();
            }
            model.QueryId = queryView.QueryViewId;
            model.QueryView = queryView;
            model.EntityId = queryView.EntityId;

            FetchDescriptor fetch = new FetchDescriptor
            {
                User = CurrentUser,
                Page = model.Page,
                PageSize = model.PageSize,
                FetchConfig = queryView.FetchConfig
            };
            if (model.IsSortBySeted)
            {
                QueryColumnSortInfo sort = new QueryColumnSortInfo(model.SortBy, model.SortDirection == 0);
                fetch.Sort = sort;
            }
            if (model.Q.IsNotEmpty())
            {
                fetch.Keyword = model.Q;
                fetch.Field = model.QField;
            }
            if (model.Filter == null)
            {
                model.Filter = new FilterExpression();
            }
            //filter disabled records
            if (model.OnlyEnabledRecords)
            {
                if (model.Filter.Conditions.IsEmpty() || model.Filter.Conditions.Find(n => n.AttributeName.IsCaseInsensitiveEqual("statecode")) == null)
                {
                    model.Filter.AddCondition("statecode", ConditionOperator.Equal, 1);
                }
            }
            //filter by relationship
            if (model.RelationShipName.IsNotEmpty())
            {
                var relationship = _relationShipFinder.FindByName(model.RelationShipName);
                if (relationship != null && model.ReferencedRecordId.HasValue && !model.ReferencedRecordId.Value.Equals(Guid.Empty))
                {
                    var condition = new ConditionExpression(relationship.ReferencingAttributeName, ConditionOperator.Equal, model.ReferencedRecordId);
                    model.Filter.AddCondition(condition);
                }
                model.RelationShipMeta = relationship;
            }
            fetch.Filter = model.Filter;

            var datas = _fetchDataService.Execute(fetch);
            model.Grid = _gridService.Build(queryView, _fetchDataService.QueryResolver.EntityList, _fetchDataService.QueryResolver.AttributeList);
            model.EntityList = _fetchDataService.QueryResolver.EntityList;
            model.AttributeList = _fetchDataService.QueryResolver.AttributeList;
            model.RelationShipList = _fetchDataService.QueryResolver.RelationShipList;
            model.Items = datas.Items;
            model.TotalItems = datas.TotalItems;
            if (model.IsSortBySeted == false)
            {
                if (_fetchDataService.QueryExpression.Orders.NotEmpty())
                {
                    model.SortBy = _fetchDataService.QueryExpression.Orders[0].AttributeName;
                    model.SortDirection = (int)_fetchDataService.QueryExpression.Orders[0].OrderType;
                }
            }

            ViewData["DialogModel"] = dm;
            return View($"~/Views/Entity/{WebContext.ActionName}.cshtml", model);
        }
    }
}
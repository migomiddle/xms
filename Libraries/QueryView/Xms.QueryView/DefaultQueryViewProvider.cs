using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Components.Platform;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.QueryView.Abstractions.Component;
using Xms.Schema.Abstractions;
using Xms.Schema.Attribute;
using Xms.Sdk.Abstractions.Query;

namespace Xms.QueryView
{
    /// <summary>
    /// 默认视图提供者
    /// </summary>
    public class DefaultQueryViewProvider : IDefaultQueryViewProvider
    {
        private readonly IDefaultAttributeProvider _defaultAttributeProvider;
        private readonly IAppContext _appContext;

        public DefaultQueryViewProvider(IAppContext appContext
            , IDefaultAttributeProvider defaultAttributeProvider)
        {
            _defaultAttributeProvider = defaultAttributeProvider;
            _appContext = appContext;
        }

        /// <summary>
        /// 生成默认视图
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public (Domain.QueryView DefaultView, List<Dependency.Domain.Dependency> Dependents) Get(Schema.Domain.Entity entity, List<Schema.Domain.Attribute> attributes)
        {
            Domain.QueryView view = new Domain.QueryView
            {
                Name = entity.LocalizedName,
                EntityId = entity.EntityId,
                EntityName = entity.Name,
                IsDefault = true,
                StateCode = Core.RecordState.Enabled,
                IsPrivate = false,
                QueryViewId = Guid.NewGuid(),
                CreatedBy = entity.CreatedBy
            };
            //fetch
            QueryExpression _queryExpression = new QueryExpression(entity.Name, _appContext.GetFeature<ICurrentUser>().UserSettings.LanguageId);
            _queryExpression.Distinct = false;
            _queryExpression.NoLock = true;
            _queryExpression.AddOrder("createdon", OrderType.Descending);
            var primaryField = attributes.Find(n => n.IsPrimaryField);
            _queryExpression.ColumnSet = new ColumnSet(primaryField.Name.ToLower(), "createdon");
            if (entity.EntityMask == EntityMaskEnum.User)
            {
                _queryExpression.ColumnSet.AddColumn("ownerid");
            }
            view.FetchConfig = _queryExpression.SerializeToJson();
            //layout
            GridDescriptor grid = new GridDescriptor();
            RowDescriptor row = new RowDescriptor();
            row.AddCell(new CellDescriptor() { Name = primaryField.Name.ToLower(), EntityName = entity.Name, IsHidden = false, IsSortable = true, Width = 150 });
            row.AddCell(new CellDescriptor() { Name = "createdon", EntityName = entity.Name, IsHidden = false, IsSortable = true, Width = 150 });
            if (entity.EntityMask == EntityMaskEnum.User)
            {
                row.AddCell(new CellDescriptor() { Name = "ownerid", EntityName = entity.Name, IsHidden = false, IsSortable = true, Width = 150 });
            }
            grid.AddRow(row);
            grid.AddSort(new QueryColumnSortInfo("createdon", false));
            view.LayoutConfig = grid.SerializeToJson(false);

            var dependents = new List<Dependency.Domain.Dependency>();
            foreach (var item in attributes.Where(x => x.Name.IsCaseInsensitiveEqual(primaryField.Name) || x.Name.IsCaseInsensitiveEqual("createdon") || x.Name.IsCaseInsensitiveEqual("ownerid")))
            {
                var dp = new Dependency.Domain.Dependency();
                //dp.DependentComponentType = DependencyComponentTypes.Get(QueryViewDefaults.ModuleName);
                dp.DependentObjectId = view.QueryViewId;
                //dp.RequiredComponentType = DependencyComponentTypes.Get(AttributeDefaults.ModuleName);
                dp.RequiredObjectId = item.AttributeId;
                dependents.Add(dp);
            }
            return (view, dependents);
        }
    }
}
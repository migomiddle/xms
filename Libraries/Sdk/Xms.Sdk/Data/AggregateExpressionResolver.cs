using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;
using Xms.Schema.Extensions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Query;

namespace Xms.Sdk.Data
{
    public class AggregateExpressionResolver : IAggregateExpressionResolver
    {
        private readonly DataRepositoryBase<dynamic> _dataRepository;

        private readonly IQueryResolverFactory _queryResolverFactory;

        public AggregateExpressionResolver(IQueryResolverFactory queryResolverFactory, IDbContext dbContext)
        {
            _queryResolverFactory = queryResolverFactory;
            _dataRepository = new DataRepositoryBase<dynamic>(dbContext);
        }

        public virtual List<dynamic> Execute(AggregateExpression agg)
        {
            var queryResolver = _queryResolverFactory.Get(agg);
            var sqlString = queryResolver.ToSqlString();
            List<string> selectFields = new List<string>();
            foreach (var a in agg.AggregateFields)
            {
                selectFields.Add(DataFieldExpressionHelper.GetAggregationExpression(a.AggregateType, a.AttributeName) + " AS " + a.AttributeName);
            }
            sqlString = string.Format("SELECT {0} FROM ({1}) a", string.Join(",", selectFields).ToLower(), sqlString);

            return _dataRepository.ExecuteQuery(sqlString, queryResolver.Parameters.Args.ToArray());
        }

        public virtual List<dynamic> Execute(QueryExpression query, Dictionary<string, AggregateType> attributeAggs, List<string> groupFields)
        {
            var queryResolver = _queryResolverFactory.Get(query);
            query.ColumnSet.Columns.Clear();
            query.ColumnSet.AddColumns(attributeAggs.Keys.ToArray());
            query.ColumnSet.AddColumns(groupFields.ToArray());
            if (query.Orders.NotEmpty())
            {
                query.Orders.Clear();
            }
            var sqlString = queryResolver.ToSqlString();
            List<string> selectFields = new List<string>();
            //List<string> groupFields = new List<string>();
            foreach (var a in attributeAggs)
            {
                selectFields.Add(DataFieldExpressionHelper.GetAggregationExpression(a.Value, a.Key) + " AS " + a.Key + (Enum.GetName(typeof(AggregateType), a.Value)));
            }
            var nameFields = new List<string>();
            foreach (var gf in groupFields)
            {
                var attr = queryResolver.AttributeList.Find(n => n.Name.IsCaseInsensitiveEqual(gf));
                var nameField = attr.GetNameField();
                if (!gf.IsCaseInsensitiveEqual(nameField))
                {
                    nameFields.Add(nameField);
                }
            }
            groupFields.AddRange(nameFields);
            sqlString = string.Format("SELECT {0} FROM ({2}) a GROUP BY {1}", string.Join(",", selectFields.Concat(groupFields)).ToLower(), string.Join(",", groupFields).ToLower(), sqlString);

            return _dataRepository.ExecuteQuery(sqlString, queryResolver.Parameters.Args.ToArray());
        }

        public virtual List<dynamic> GroupingTop(int top, QueryExpression query, string groupField, OrderExpression order)
        {
            var queryResolver = _queryResolverFactory.Get(query);
            var orderString = string.Format(" ORDER BY {0}.{1} ", query.EntityName, order.AttributeName + (order.OrderType == OrderType.Descending ? " DESC" : ""));
            query.Orders.Clear();
            var sqlString = queryResolver.ToSqlString();
            var rowSql = string.Format("ROW_NUMBER() OVER(PARTITION BY {2}.{0} {1}) as RowNum", groupField, orderString, query.EntityName);
            sqlString = sqlString.Replace("SELECT ", "SELECT " + rowSql + ",");
            sqlString = string.Format("SELECT * FROM ({0}) a ORDER BY RowNum", sqlString);

            return _dataRepository.ExecuteQuery(sqlString, queryResolver.Parameters.Args.ToArray());
        }
    }
}
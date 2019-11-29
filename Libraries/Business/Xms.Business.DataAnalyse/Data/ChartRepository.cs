using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Business.DataAnalyse.Domain;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data;
using Xms.Infrastructure.Utility;
using Xms.Schema.Data;
using Xms.Schema.Domain;
using Xms.Schema.Extensions;
using Xms.Sdk.Abstractions.Query;
using Xms.Sdk.Data;
using Xms.Sdk.Query;

namespace Xms.Business.DataAnalyse.Data
{
    /// <summary>
    /// 图表数据仓储
    /// </summary>
    public class ChartRepository : DefaultRepository<Chart>, IChartRepository
    {
        private readonly IAttributeRepository _attributeRepository;
        private readonly IStringMapRepository _stringMapRepository;
        private readonly IOptionSetDetailRepository _optionSetDetailRepository;
        private readonly DataRepositoryBase<dynamic> _dataRepository;

        public ChartRepository(IDbContext dbContext
            , IAttributeRepository attributeRepository
            , IStringMapRepository stringMapRepository
            , IOptionSetDetailRepository optionSetDetailRepository
            ) : base(dbContext)
        {
            _dataRepository = new DataRepositoryBase<dynamic>(dbContext);
            _attributeRepository = attributeRepository;
            _stringMapRepository = stringMapRepository;
            _optionSetDetailRepository = optionSetDetailRepository;
        }

        #region implements

        /// <summary>
        /// 获取图表需要的数据
        /// </summary>
        /// <param name="chartData"></param>
        /// <param name="query"></param>
        /// <param name="queryResolver"></param>
        /// <param name="attributes"></param>
        /// <returns></returns>
        public List<dynamic> GetChartDataSource(ChartDataDescriptor chartData, QueryExpression query, IQueryResolver queryResolver)
        {
            query.Orders.Clear();//清除视图原有的排序
            query.ColumnSet.AllColumns = false;//清除视图原有的列
            query.ColumnSet.Columns.Clear();
            var invalidLinkEntities = new List<LinkEntity>();//不需要的关联实体
            if (query.LinkEntities.NotEmpty())
            {
                foreach (var le in query.LinkEntities)
                {
                    if (le.LinkCriteria == null || (le.LinkCriteria.Filters.IsEmpty() && le.LinkCriteria.Conditions.IsEmpty()))
                    {
                        invalidLinkEntities.Add(le);
                    }
                    le.Columns.AllColumns = false;
                    le.Columns.Columns.Clear();
                }
            }
            if (invalidLinkEntities.NotEmpty())
            {
                foreach (var item in invalidLinkEntities)
                {
                    query.LinkEntities.Remove(item);
                }
            }
            //统计类型：汇总、平均值、最大值、最小值
            //X轴、分类
            //group by 日期、状态等，select count(@field),avg(@field),max(@field),min(@field) from (@sqlstring) group by @field
            //系列（多条）、数据
            //获取生成的sqlstring，select count(@field),avg(@field),max(@field),min(@field) from (@sqlstring)
            var attrbuteNames = chartData.Fetch.Select(f => f.Attribute);
            var attributes = _attributeRepository.Query(x => x.EntityName == query.EntityName && x.Name.In(attrbuteNames)).ToList();
            var selectString = new List<string>();
            var groupbyString = new List<string>();
            var orderbyString = new List<string>();
            var topCount = -1;
            foreach (var item in chartData.Fetch)
            {
                var attr = attributes.Find(n => n.Name.IsCaseInsensitiveEqual(item.Attribute));
                var name = item.Attribute;
                if (item.Type == ChartItemType.Series)
                {
                    if (item.TopCount.HasValue && item.TopCount.Value > 0)
                    {
                        if (item.TopCount.Value > topCount)
                        {
                            topCount = item.TopCount.Value;
                        }
                        orderbyString.Add(item.Attribute + (Enum.GetName(typeof(AggregateType), item.Aggregate)) + (item.TopDirection == TopDirectionType.Desc ? " desc" : ""));
                    }
                    selectString.Add(string.Format("{0} AS {1}", DataFieldExpressionHelper.GetAggregationExpression(item.Aggregate, name), item.Attribute + (Enum.GetName(typeof(AggregateType), item.Aggregate))));
                }
                else if (item.Type == ChartItemType.Category)
                {
                    var groupField = string.Empty;
                    if (attr.TypeIsDateTime() && item.DateGrouping.HasValue)//date group
                    {
                        groupField = DataFieldExpressionHelper.GetDateGroupingExpression(item.DateGrouping.Value, item.Attribute);
                    }
                    else
                    {
                        groupField = name;
                    }
                    if (!groupbyString.Exists(n => n.IsCaseInsensitiveEqual(groupField)))
                    {
                        groupbyString.Add(groupField);
                    }
                    selectString.Add(groupField + " AS " + name);
                    if (attr.TypeIsPrimaryKey())
                    {
                        var primaryField = _attributeRepository.Find(n => n.EntityName == query.EntityName && n.IsPrimaryField == true);
                        query.ColumnSet.AddColumn(primaryField.Name);
                        selectString.Add(string.Format("{0} AS {1}name", primaryField.Name, item.Attribute));
                        groupbyString.Add(primaryField.Name);
                        attributes.Add(primaryField);
                    }
                    else if (attr.TypeIsLookUp() || attr.TypeIsOwner() || attr.TypeIsCustomer())
                    {
                        query.ColumnSet.AddColumn(name + "name");
                        groupbyString.Add(name + "name");
                        selectString.Add(name + "name");
                    }
                }
                if (!query.ColumnSet.Columns.Exists(n => n.IsCaseInsensitiveEqual(name)))
                {
                    query.ColumnSet.AddColumn(name);
                }
                if (attr.TypeIsPickList() || attr.TypeIsStatus())
                {
                    attr.OptionSet = new OptionSet();
                    attr.OptionSet.Items = _optionSetDetailRepository.Query(x => x.OptionSetId == attr.OptionSetId.Value).ToList();
                }
                else if (attr.TypeIsState() || attr.TypeIsBit())
                {
                    attr.PickLists = _stringMapRepository.Query(f => f.AttributeId == attr.AttributeId)?.ToList();
                }
                attributes.Add(attr);
                //如果不指定前后X项，则默认按日期的正序排序
                if (topCount <= 0 && attr.TypeIsDateTime())
                {
                    orderbyString.Add(attr.Name + " ASC");
                }
            }
            queryResolver.AttributeList = attributes;
            var sqlstring = queryResolver.ToSqlString();
            sqlstring = string.Format("SELECT {2} {0} FROM (" + sqlstring + ") a GROUP BY {1} {3}"
                , string.Join(",", selectString)
                , string.Join(",", groupbyString)
                , topCount > 0 ? " TOP " + topCount : string.Empty
                , (orderbyString.NotEmpty() ? " ORDER BY " + string.Join(",", orderbyString) : string.Empty)
                );

            return _dataRepository.ExecuteQuery(sqlstring, queryResolver.Parameters.Args.ToArray());
        }

        public PagedList<Domain.Chart> QueryPaged(QueryDescriptor<Domain.Chart> q, int solutionComponentType, Guid solutionId, bool existInSolution)
        {
            if (q.QueryText.IsNotEmpty())
            {
                q.QueryText += " AND ";
            }
            q.QueryText += "ChartId " + (existInSolution ? "" : "NOT") + " IN(SELECT ObjectId FROM SolutionComponent WHERE SolutionId=@" + q.Parameters.Count;
            q.Parameters.Add(new QueryParameter("@" + q.Parameters.Count, solutionId));
            q.QueryText += " and ComponentType = @" + q.Parameters.Count;
            q.Parameters.Add(new QueryParameter("@" + q.Parameters.Count, solutionComponentType));
            q.QueryText += ")";
            return base.QueryPaged(q);
        }

        #endregion implements
    }
}
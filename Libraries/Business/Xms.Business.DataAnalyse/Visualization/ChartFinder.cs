using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Business.DataAnalyse.Data;
using Xms.Business.DataAnalyse.Domain;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Dependency.Abstractions;
using Xms.Module.Core;

namespace Xms.Business.DataAnalyse.Visualization
{
    /// <summary>
    /// 图表查找服务
    /// </summary>
    public class ChartFinder : IChartFinder, IDependentLookup<Chart>
    {
        private readonly IChartRepository _chartRepository;

        public ChartFinder(IChartRepository chartRepository)
        {
            _chartRepository = chartRepository;
        }

        public Chart FindById(Guid id)
        {
            var data = _chartRepository.FindById(id);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public Chart Find(Expression<Func<Chart, bool>> predicate)
        {
            var data = _chartRepository.Find(predicate);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public PagedList<Chart> QueryPaged(Func<QueryDescriptor<Chart>, QueryDescriptor<Chart>> container)
        {
            QueryDescriptor<Chart> q = container(QueryDescriptorBuilder.Build<Chart>());
            var datas = _chartRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<Chart> QueryPaged(Func<QueryDescriptor<Chart>, QueryDescriptor<Chart>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<Chart> q = container(QueryDescriptorBuilder.Build<Chart>());
            var datas = _chartRepository.QueryPaged(q, Module.Core.ModuleCollection.GetIdentity(ChartDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Chart> Query(Func<QueryDescriptor<Chart>, QueryDescriptor<Chart>> container)
        {
            QueryDescriptor<Chart> q = container(QueryDescriptorBuilder.Build<Chart>());
            var datas = _chartRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        #region dependency

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.Name } : null;
        }

        public int ComponentType => ModuleCollection.GetIdentity(ChartDefaults.ModuleName);

        #endregion dependency

        private void WrapLocalizedLabel(IEnumerable<Chart> datas)
        {
            /*
            if (datas.NotEmpty())
            {
                var ids = datas.Select(f => f.ChartId);
                var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == _user.UserSettings.LanguageId && f.ObjectId.In(ids)));
                foreach (var d in datas)
                {
                    d.Name = _localizedLabelService.GetLabelText(labels, d.ChartId, "LocalizedName", d.Name);
                    d.Description = _localizedLabelService.GetLabelText(labels, d.ChartId, "Description", d.Description);
                }
            }
            */
        }

        private void WrapLocalizedLabel(Chart entity)
        {
            /*
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == _user.UserSettings.LanguageId && f.ObjectId == entity.ChartId));
            entity.Name = _localizedLabelService.GetLabelText(labels, entity.ChartId, "LocalizedName", entity.Name);
            entity.Description = _localizedLabelService.GetLabelText(labels, entity.ChartId, "Description", entity.Description);
            */
        }
    }
}
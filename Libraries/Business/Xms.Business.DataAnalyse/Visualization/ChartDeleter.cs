using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Business.DataAnalyse.Data;
using Xms.Business.DataAnalyse.Domain;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Dependency;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;

namespace Xms.Business.DataAnalyse.Visualization
{
    /// <summary>
    /// 图表删除服务
    /// </summary>
    public class ChartDeleter : IChartDeleter, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly IChartRepository _chartRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IChartDependency _dependencyService;
        private readonly IDependencyChecker _dependencyChecker;

        public ChartDeleter(IChartRepository chartRepository
            , ILocalizedLabelService localizedLabelService
            , IChartDependency dependencyService
            , IDependencyChecker dependencyChecker)
        {
            _chartRepository = chartRepository;
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
            _dependencyChecker = dependencyChecker;
        }

        /// <summary>
        /// 实体级联删除
        /// </summary>
        /// <param name="parent">被删除的实体</param>
        public void CascadeDelete(params Schema.Domain.Entity[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var entityIds = parent.Select(x => x.EntityId).ToArray();
            var deleteds = _chartRepository.Query(x => x.EntityId.In(entityIds));
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds, null);
            }
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            var deleteds = _chartRepository.Query(x => x.ChartId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds, (deleted) =>
                {
                    //检查依赖项
                    _dependencyChecker.CheckAndThrow<Chart>(ChartDefaults.ModuleName, deleted.ChartId);
                    return true;
                });
            }
            return result;
        }

        private bool DeleteCore(IEnumerable<Chart> deleteds, Func<Chart, bool> validation)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = true;
            if (validation != null)
            {
                foreach (var deleted in deleteds)
                {
                    result = validation?.Invoke(deleted) ?? true;
                }
            }
            if (result)
            {
                var ids = deleteds.Select(x => x.ChartId);
                using (UnitOfWork.Build(_chartRepository.DbContext))
                {
                    result = _chartRepository.DeleteMany(ids);
                    //删除依赖项
                    _dependencyService.Delete(ids.ToArray());
                    //localization
                    _localizedLabelService.DeleteByObject(ids.ToArray());
                }
            }
            return result;
        }
    }
}
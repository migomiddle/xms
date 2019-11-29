using System;
using System.Linq;
using Xms.Business.SerialNumber.Data;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Localization.Abstractions;
using Xms.Plugin;
using Xms.Solution;

namespace Xms.Business.SerialNumber
{
    /// <summary>
    /// 自动编号规则删除服务
    /// </summary>
    public class SerialNumberRuleDeleter : ISerialNumberRuleDeleter, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly ISerialNumberRuleRepository _serialNumberRuleRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ILocalizedTextProvider _loc;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly ISerialNumberDependency _dependencyService;
        private readonly IEntityPluginDeleter _entityPluginDeleter;
        private readonly Caching.CacheManager<Domain.SerialNumberRule> _cacheService;
        private readonly IAppContext _appContext;

        public SerialNumberRuleDeleter(IAppContext appContext
            , ISerialNumberRuleRepository serialNumberRuleRepository
            , ILocalizedLabelService localizedLabelService
            , ISolutionComponentService solutionComponentService
            , ISerialNumberDependency dependencyService
            , IEntityPluginDeleter entityPluginDeleter)
        {
            _appContext = appContext;
            _serialNumberRuleRepository = serialNumberRuleRepository;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _dependencyService = dependencyService;
            _entityPluginDeleter = entityPluginDeleter;
            _cacheService = new Caching.CacheManager<Domain.SerialNumberRule>(SerialNumberRuleCache.CacheKey(_appContext), _appContext.PlatformSettings.CacheEnabled);
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的实体</param>
        public void CascadeDelete(params Schema.Domain.Entity[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var entityIds = parent.Select(x => x.EntityId).ToArray();
            var deleteds = _serialNumberRuleRepository.Query(x => x.EntityId.In(entityIds));
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds.ToArray());
            }
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = true;
            var deleteds = _serialNumberRuleRepository.Query(x => x.SerialNumberRuleId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds.ToArray());
            }
            return result;
        }

        private bool DeleteCore(params Domain.SerialNumberRule[] deleteds)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = true;
            var ids = deleteds.Select(x => x.SerialNumberRuleId).ToArray();
            using (UnitOfWork.Build(_serialNumberRuleRepository.DbContext))
            {
                result = _serialNumberRuleRepository.DeleteMany(ids);
                //删除依赖项
                _dependencyService.Delete(ids);
                //localization
                _localizedLabelService.DeleteByObject(ids);
                //solution component
                _solutionComponentService.DeleteObject(deleteds.First().SolutionId, SerialNumberRuleDefaults.ModuleName, ids);
                var entityIds = deleteds.Select(x => x.EntityId).Distinct().ToArray();
                //plugin
                foreach (var eid in entityIds)
                {
                    _entityPluginDeleter.DeleteByEntityId(eid);
                }
                foreach (var deleted in deleteds)
                {
                    //remove from cache
                    _cacheService.RemoveEntity(deleted);
                }
            }
            return result;
        }
    }
}
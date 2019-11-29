using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Business.DuplicateValidator.Data;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Plugin;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复检测规则删除服务
    /// </summary>
    public class DuplicateRuleDeleter : IDuplicateRuleDeleter, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly IDuplicateRuleRepository _duplicateRuleRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IDuplicateRuleDependency _dependencyService;
        private readonly Caching.CacheManager<DuplicateRule> _cacheService;
        private readonly IEntityPluginDeleter _entityPluginDeleter;
        private readonly IEnumerable<ICascadeDelete<DuplicateRule>> _cascadeDeletes;
        private readonly IAppContext _appContext;

        public DuplicateRuleDeleter(IAppContext appContext
            , IDuplicateRuleRepository duplicateRuleRepository
            , ILocalizedLabelService localizedLabelService
            , IDuplicateRuleDependency dependencyService
            , IEntityPluginDeleter entityPluginDeleter
            , IEnumerable<ICascadeDelete<DuplicateRule>> cascadeDeletes)
        {
            _appContext = appContext;
            _duplicateRuleRepository = duplicateRuleRepository;
            _localizedLabelService = localizedLabelService;
            _dependencyService = dependencyService;
            _entityPluginDeleter = entityPluginDeleter;
            _cascadeDeletes = cascadeDeletes;
            _cacheService = new Caching.CacheManager<DuplicateRule>(_appContext.OrganizationUniqueName + ":duplicaterules", _appContext.PlatformSettings.CacheEnabled);
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
            var deleteds = _duplicateRuleRepository.Query(x => x.EntityId.In(entityIds));
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds.ToArray());
            }
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            var deleteds = _duplicateRuleRepository.Query(x => x.DuplicateRuleId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds.ToArray());
            }
            return result;
        }

        private bool DeleteCore(params DuplicateRule[] deleteds)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = false;
            var ids = deleteds.Select(x => x.DuplicateRuleId).ToArray();
            using (UnitOfWork.Build(_duplicateRuleRepository.DbContext))
            {
                //cascade delete
                _cascadeDeletes?.ToList().ForEach((x) => { x.CascadeDelete(deleteds.ToArray()); });
                result = _duplicateRuleRepository.DeleteMany(ids);
                //删除依赖项
                _dependencyService.Delete(ids);
                //localization
                _localizedLabelService.DeleteByObject(ids);
                var entityIds = deleteds.Select(x => x.EntityId).Distinct().ToArray();
                //plugin
                foreach (var eid in entityIds)
                {
                    _entityPluginDeleter.DeleteByEntityId(eid);
                }
                foreach (var item in deleteds)
                {
                    //remove from cache
                    _cacheService.SetEntity(item);
                }
            }
            return result;
        }
    }
}
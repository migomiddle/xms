using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Dependency;
using Xms.Infrastructure;
using Xms.Localization;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;
using Xms.Solution;

namespace Xms.Schema.Entity
{
    /// <summary>
    /// 实体删除服务
    /// </summary>
    public class EntityDeleter : IEntityDeleter
    {
        private readonly IEntityRepository _entityRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly Caching.CacheManager<Domain.Entity> _cacheService;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IDependencyService _dependencyService;
        private readonly IDependencyChecker _dependencyChecker;
        private readonly IEnumerable<ICascadeDelete<Domain.Entity>> _cascadeDeletes;
        private readonly IAppContext _appContext;

        public EntityDeleter(IAppContext appContext
            , IEntityRepository entityRepository
            , ISolutionComponentService solutionComponentService
            , ILocalizedLabelService localizedLabelService
            , IDependencyService dependencyService
            , IDependencyChecker dependencyChecker
            , IEnumerable<ICascadeDelete<Domain.Entity>> cascadeDeletes
            )
        {
            _appContext = appContext;
            _entityRepository = entityRepository;
            _localizedLabelService = localizedLabelService;
            _cacheService = new Caching.CacheManager<Domain.Entity>(_appContext.OrganizationUniqueName + ":entities", _appContext.PlatformSettings.CacheEnabled);
            _solutionComponentService = solutionComponentService;
            _dependencyService = dependencyService;
            _dependencyChecker = dependencyChecker;
            _cascadeDeletes = cascadeDeletes;
        }

        public bool DeleteById(bool deleteTable = true, params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            using (UnitOfWork.Build(_entityRepository.DbContext))
            {
                foreach (var item in id)
                {
                    var deleted = _entityRepository.FindById(item);
                    if (deleted == null || !deleted.IsCustomizable)
                    {
                        continue;
                    }
                    //检查依赖项
                    _dependencyChecker.CheckAndThrow<Domain.Entity>(EntityDefaults.ModuleName, item);
                    //cascade delete
                    _cascadeDeletes?.ToList().ForEach((x) => { x.CascadeDelete(deleted); });
                    result = _entityRepository.DeleteById(item, deleteTable);
                    //删除依赖项
                    _dependencyService.DeleteByDependentId(EntityDefaults.ModuleName, item);
                    //solution component
                    _solutionComponentService.DeleteObject(deleted.SolutionId, deleted.EntityId, EntityDefaults.ModuleName);
                    //entity localization
                    _localizedLabelService.DeleteByObject(deleted.EntityId);
                    //remove from cache
                    _cacheService.RemoveEntity(deleted);
                }
            }
            return result;
        }
    }
}
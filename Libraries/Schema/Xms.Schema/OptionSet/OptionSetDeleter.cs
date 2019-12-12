using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Dependency;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Localization.Abstractions;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;
using Xms.Schema.Extensions;
using Xms.Solution;

namespace Xms.Schema.OptionSet
{
    /// <summary>
    /// 选项集删除服务
    /// </summary>
    public class OptionSetDeleter : IOptionSetDeleter, ICascadeDelete<Domain.Attribute>
    {
        private readonly IOptionSetRepository _optionSetRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ILocalizedTextProvider _loc;
        private readonly Caching.CacheManager<Domain.OptionSet> _cacheService;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IOptionSetDetailDeleter _optionSetDetailDeleter;
        private readonly IDependencyService _dependencyService;
        private readonly IDependencyChecker _dependencyChecker;
        private readonly IEnumerable<ICascadeDelete<Domain.OptionSet>> _cascadeDeletes;
        private readonly IAppContext _appContext;

        public OptionSetDeleter(IAppContext appContext
            , IOptionSetRepository optionSetRepository
            , ISolutionComponentService solutionComponentService
            , IOptionSetDetailDeleter optionSetDetailDeleter
            , ILocalizedLabelService localizedLabelService
            , IDependencyService dependencyService
            , IDependencyChecker dependencyChecker
            , IEnumerable<ICascadeDelete<Domain.OptionSet>> cascadeDeletes)
        {
            _appContext = appContext;
            _optionSetRepository = optionSetRepository;
            _loc = _appContext.GetFeature<ILocalizedTextProvider>();
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _optionSetDetailDeleter = optionSetDetailDeleter;
            _dependencyService = dependencyService;
            _dependencyChecker = dependencyChecker;
            _cascadeDeletes = cascadeDeletes;
            _cacheService = new Caching.CacheManager<Domain.OptionSet>(_appContext.OrganizationUniqueName + ":optionsets", _appContext.PlatformSettings.CacheEnabled);
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的字段</param>
        public void CascadeDelete(params Domain.Attribute[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var ids = parent.Where(x => (x.TypeIsPickList() || x.TypeIsStatus()) && x.OptionSetId.HasValue && !x.OptionSetId.Value.Equals(Guid.Empty))
                ?.Select(x => x.OptionSetId.Value).ToList();
            if (ids.IsEmpty())
            {
                return;
            }
            //删除字段关联的选项集，非公共的
            var deleteds = _optionSetRepository.Query(x => x.OptionSetId.In(ids) && x.IsPublic == false);
            if (deleteds.NotEmpty())
            {
                DeleteCore(deleteds, null);
            }
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            var deleteds = _optionSetRepository.Query(x => x.OptionSetId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds, (deleted) =>
                {
                    //检查依赖项
                    _dependencyChecker.CheckAndThrow<Domain.OptionSet>(OptionSetDefaults.ModuleName, deleted.OptionSetId);
                    return true;
                });
            }
            return result;
        }

        private bool DeleteCore(IEnumerable<Domain.OptionSet> deleteds, Func<Domain.OptionSet, bool> validation)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = true;
            foreach (var deleted in deleteds)
            {
                result = validation?.Invoke(deleted) ?? true;
            }
            if (result)
            {
                var ids = deleteds.Select(x => x.OptionSetId).ToArray();
                using (UnitOfWork.Build(_optionSetRepository.DbContext))
                {
                    //cascade delete
                    _cascadeDeletes?.ToList().ForEach((x) => { x.CascadeDelete(deleteds.ToArray()); });
                    result = _optionSetRepository.DeleteMany(ids);
                    //删除依赖项
                    _dependencyService.DeleteByDependentId(OptionSetDefaults.ModuleName, ids);
                    //localization
                    _localizedLabelService.DeleteByObject(ids);
                    foreach (var deleted in deleteds)
                    {
                        //solution component
                        _solutionComponentService.DeleteObject(deleted.SolutionId, deleted.OptionSetId, OptionSetDefaults.ModuleName);
                        //remove from cache
                        _cacheService.RemoveEntity(deleted);
                    }
                }
            }
            return result;
        }
    }
}
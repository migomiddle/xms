using System;
using Xms.Business.DuplicateValidator.Data;
using Xms.Business.DuplicateValidator.Domain;
using Xms.Context;
using Xms.Core;
using Xms.Data.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Plugin;
using Xms.Plugin.Domain;
using Xms.Solution.Abstractions;

namespace Xms.Business.DuplicateValidator
{
    /// <summary>
    /// 重复检测规则创建服务
    /// </summary>
    public class DuplicateRuleCreater : IDuplicateRuleCreater
    {
        private readonly IDuplicateRuleRepository _duplicateRuleRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IDuplicateRuleConditionService _duplicateRuleConditionService;
        private readonly IDuplicateRuleDependency _dependencyService;
        private readonly IEntityPluginCreater _entityPluginCreater;
        private readonly Caching.CacheManager<DuplicateRule> _cacheService;
        private readonly IAppContext _appContext;

        public DuplicateRuleCreater(IAppContext appContext
            , IDuplicateRuleRepository duplicateRuleRepository
            , IDuplicateRuleConditionService duplicateRuleConditionService
            , ILocalizedLabelService localizedLabelService
            , IDuplicateRuleDependency dependencyService
            , IEntityPluginCreater entityPluginCreater)
        {
            _appContext = appContext;
            _duplicateRuleRepository = duplicateRuleRepository;
            _localizedLabelService = localizedLabelService;
            _duplicateRuleConditionService = duplicateRuleConditionService;
            _dependencyService = dependencyService;
            _entityPluginCreater = entityPluginCreater;
            _cacheService = new Caching.CacheManager<DuplicateRule>(_appContext.OrganizationUniqueName + ":duplicaterules", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Create(DuplicateRule entity)
        {
            entity.SolutionId = SolutionDefaults.DefaultSolutionId;//组件属于默认解决方案
            var result = false;
            using (UnitOfWork.Build(_duplicateRuleRepository.DbContext))
            {
                result = _duplicateRuleRepository.Create(entity);
                //检测条件
                if (entity.Conditions.NotEmpty())
                {
                    foreach (var item in entity.Conditions)
                    {
                        item.EntityId = entity.EntityId;
                    }
                    result = _duplicateRuleConditionService.CreateMany(entity.Conditions);
                }
                //依赖于字段
                _dependencyService.Create(entity);
                //本地化标签
                _localizedLabelService.Create(entity.SolutionId, entity.Name.IfEmpty(""), DuplicateRuleDefaults.ModuleName, "LocalizedName", entity.DuplicateRuleId, this._appContext.BaseLanguage);
                _localizedLabelService.Create(entity.SolutionId, entity.Description.IfEmpty(""), DuplicateRuleDefaults.ModuleName, "Description", entity.DuplicateRuleId, this._appContext.BaseLanguage);
                //plugin
                _entityPluginCreater.Create(new EntityPlugin()
                {
                    AssemblyName = DuplicateRuleDefaults.AssemblyName
                    ,
                    ClassName = DuplicateRuleDefaults.PluginClassName
                    ,
                    EntityId = entity.EntityId
                    ,
                    EventName = Enum.GetName(typeof(OperationTypeEnum), OperationTypeEnum.Create)
                    ,
                    IsVisibled = false
                    ,
                    TypeCode = 0
                    ,
                    StateCode = RecordState.Enabled
                });
                //add to cache
                _cacheService.SetEntity(entity);
            }
            return result;
        }
    }
}
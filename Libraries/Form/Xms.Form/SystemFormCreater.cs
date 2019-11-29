using System;
using Xms.Context;
using Xms.Data.Abstractions;
using Xms.Form.Abstractions;
using Xms.Form.Data;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Solution;

namespace Xms.Form
{
    /// <summary>
    /// 表单创建服务
    /// </summary>
    public class SystemFormCreater : ISystemFormCreater
    {
        private readonly ISystemFormRepository _systemFormRepository;
        private readonly IDefaultSystemFormProvider _defaultSystemFormProvider;
        private readonly ILocalizedLabelBatchBuilder _localizedLabelService;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IFormService _formService;
        private readonly ISystemFormDependency _dependencyService;
        private readonly Caching.CacheManager<Domain.SystemForm> _cacheService;
        private readonly IAppContext _appContext;

        public SystemFormCreater(IAppContext appContext
            , ISystemFormRepository systemFormRepository
            , ILocalizedLabelBatchBuilder localizedLabelService
            , ISolutionComponentService solutionComponentService
            , IFormService formService
            , ISystemFormDependency dependencyService
            , IDefaultSystemFormProvider defaultSystemFormProvider)
        {
            _appContext = appContext;
            _systemFormRepository = systemFormRepository;
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _formService = formService;
            _dependencyService = dependencyService;
            _defaultSystemFormProvider = defaultSystemFormProvider;
            _cacheService = new Caching.CacheManager<Domain.SystemForm>(_appContext.OrganizationUniqueName + ":systemforms", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool Create(Domain.SystemForm entity)
        {
            return CreateCore(entity, (n) =>
            {
                _dependencyService.Create(entity);
            });
        }

        public bool CreateDefaultForm(Schema.Domain.Entity entity)
        {
            var (DefaultForm, Dependents) = _defaultSystemFormProvider.Get(entity);
            return this.Create(DefaultForm);
        }

        private bool CreateCore(Domain.SystemForm entity, Action<Domain.SystemForm> createDependents)
        {
            if (entity.FormConfig.IsEmpty())
            {
                return false;
            }
            entity.OrganizationId = _appContext.OrganizationId;
            _formService.Init(entity);
            var result = true;
            using (UnitOfWork.Build(_systemFormRepository.DbContext))
            {
                result = _systemFormRepository.Create(entity);
                //依赖项
                createDependents?.Invoke(entity);
                //本地化标签
                _localizedLabelService.Append(entity.SolutionId, entity.Name.IfEmpty(""), entity.FormType == (int)FormType.Dashboard ? DashBoardDefaults.ModuleName : FormDefaults.ModuleName, "LocalizedName", entity.SystemFormId)
                .Append(entity.SolutionId, entity.Description.IfEmpty(""), entity.FormType == (int)FormType.Dashboard ? DashBoardDefaults.ModuleName : FormDefaults.ModuleName, "Description", entity.SystemFormId)
                .Save();
                _formService.UpdateLocalizedLabel(null);
                if (entity.FormType == (int)FormType.Dashboard)
                {
                    //solution component
                    result = _solutionComponentService.Create(entity.SolutionId, entity.SystemFormId, DashBoardDefaults.ModuleName);
                }
                //add to cache
                _cacheService.SetEntity(_systemFormRepository.FindById(entity.SystemFormId));
            }
            return result;
        }
    }
}
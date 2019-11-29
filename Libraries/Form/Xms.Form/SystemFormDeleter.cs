using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Dependency;
using Xms.Event.Abstractions;
using Xms.Form.Abstractions;
using Xms.Form.Data;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Solution;

namespace Xms.Form
{
    /// <summary>
    /// 表单删除服务
    /// </summary>
    public class SystemFormDeleter : ISystemFormDeleter, ICascadeDelete<Schema.Domain.Entity>
    {
        private readonly ISystemFormRepository _systemFormRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly ISolutionComponentService _solutionComponentService;
        private readonly IFormService _formService;
        private readonly ISystemFormDependency _dependencyService;
        private readonly IDependencyChecker _dependencyChecker;
        private readonly IEventPublisher _eventPublisher;
        private readonly IEnumerable<ICascadeDelete<Domain.SystemForm>> _cascadeDeletes;
        private readonly Caching.CacheManager<Domain.SystemForm> _cacheService;
        private readonly IAppContext _appContext;

        public SystemFormDeleter(IAppContext appContext
            , ISystemFormRepository systemFormRepository
            , ILocalizedLabelService localizedLabelService
            , ISolutionComponentService solutionComponentService
            , IFormService formService
            , ISystemFormDependency dependencyService
            , IDependencyChecker dependencyChecker
            , IEventPublisher eventPublisher
            , IEnumerable<ICascadeDelete<Domain.SystemForm>> cascadeDeletes)
        {
            _appContext = appContext;
            _systemFormRepository = systemFormRepository;
            _localizedLabelService = localizedLabelService;
            _solutionComponentService = solutionComponentService;
            _formService = formService;
            _dependencyService = dependencyService;
            _dependencyChecker = dependencyChecker;
            _eventPublisher = eventPublisher;
            _cascadeDeletes = cascadeDeletes;
            _cacheService = new Caching.CacheManager<Domain.SystemForm>(_appContext.OrganizationUniqueName + ":systemforms", _appContext.PlatformSettings.CacheEnabled);
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            var deleteds = _systemFormRepository.Query(x => x.SystemFormId.In(id));
            if (deleteds.NotEmpty())
            {
                result = DeleteCore(deleteds, (deleted) =>
                {
                    if (deleted.IsDefault)
                    {
                        return false;
                    }
                    //检查依赖项
                    _dependencyChecker.CheckAndThrow<Schema.Domain.Entity>(FormDefaults.ModuleName, deleted.SystemFormId);
                    return true;
                });
            }
            return result;
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
            var forms = _systemFormRepository.Query(x => x.EntityId.In(entityIds));
            if (forms.NotEmpty())
            {
                DeleteCore(forms, null);
            }
        }

        private bool DeleteCore(IEnumerable<Domain.SystemForm> deleteds, Func<Domain.SystemForm, bool> validation)
        {
            Guard.NotEmpty(deleteds, nameof(deleteds));
            var result = true;
            foreach (var deleted in deleteds)
            {
                result = validation?.Invoke(deleted) ?? true;
            }
            if (result)
            {
                var ids = deleteds.Select(x => x.SystemFormId).ToArray();
                using (UnitOfWork.Build(_systemFormRepository.DbContext))
                {
                    result = _systemFormRepository.DeleteMany(ids);
                    //删除依赖项
                    _dependencyService.Delete(ids);
                    foreach (var deleted in deleteds)
                    {
                        if (deleted.FormType == (int)FormType.Dashboard)
                        {
                            //solution component
                            result = _solutionComponentService.DeleteObject(deleted.SolutionId, deleted.SystemFormId, DashBoardDefaults.ModuleName);
                        }
                        //localization
                        _localizedLabelService.DeleteByObject(ids);
                        _formService.Init(deleted).DeleteOriginalLabels(deleted);
                        _eventPublisher.Publish(new ObjectDeletedEvent<Domain.SystemForm>(deleted.FormType == (int)FormType.Dashboard ? DashBoardDefaults.ModuleName : FormDefaults.ModuleName, deleted));
                        //remove from cache
                        _cacheService.RemoveEntity(deleted);
                    }
                    //cascade delete
                    _cascadeDeletes?.ToList().ForEach((x) => { x.CascadeDelete(deleteds.ToArray()); });
                }
            }
            return result;
        }
    }
}
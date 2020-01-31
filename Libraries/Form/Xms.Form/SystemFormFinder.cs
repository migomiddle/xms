using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Dependency.Abstractions;
using Xms.Form.Abstractions;
using Xms.Form.Data;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;

namespace Xms.Form
{
    /// <summary>
    /// 表单查询服务
    /// </summary>
    public class SystemFormFinder : ISystemFormFinder, IDependentLookup<Domain.SystemForm>
    {
        private readonly ISystemFormRepository _systemFormRepository;
        private readonly IRoleObjectAccessService _roleObjectAccessService;
        private readonly Caching.CacheManager<Domain.SystemForm> _cacheService;
        private readonly IAppContext _appContext;
        private readonly ICurrentUser _currentUser;

        public SystemFormFinder(IAppContext appContext
            , ISystemFormRepository systemFormRepository
            , IRoleObjectAccessService roleObjectAccessService)
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _systemFormRepository = systemFormRepository;
            _roleObjectAccessService = roleObjectAccessService;
            _cacheService = new Caching.CacheManager<Domain.SystemForm>(_appContext.OrganizationUniqueName + ":systemforms", _appContext.PlatformSettings.CacheEnabled);
        }

        public Domain.SystemForm FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("SystemFormId", id.ToString());

            Domain.SystemForm entity = _cacheService.Get(dic, () =>
             {
                 return _systemFormRepository.FindById(id);
             });
            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public Domain.SystemForm FindEntityDefaultForm(Guid entityId)
        {
            List<Domain.SystemForm> items = _cacheService.GetVersionItems(entityId.ToString(), () =>
             {
                 return _systemFormRepository.Query(f => f.EntityId == entityId)?.ToList();
             });
            Domain.SystemForm defaultForm = null;
            if (items.NotEmpty())
            {
                defaultForm = items.FirstOrDefault(n => n.IsDefault == true);
            }
            if (defaultForm != null)
            {
                WrapLocalizedLabel(defaultForm);
            }
            return defaultForm;
        }

        public Domain.SystemForm FindEntityDefaultForm(string entityName)
        {
            List<Domain.SystemForm> items = _cacheService.GetVersionItems(entityName, () =>
            {
                return _systemFormRepository.Query(f => f.EntityName == entityName)?.ToList();
            });
            Domain.SystemForm defaultForm = null;
            if (items.NotEmpty())
            {
                defaultForm = items.FirstOrDefault(n => n.IsDefault == true);
            }
            if (defaultForm != null)
            {
                WrapLocalizedLabel(defaultForm);
            }
            return defaultForm;
        }

        public Domain.SystemForm Find(Expression<Func<Domain.SystemForm, bool>> predicate)
        {
            var data = _systemFormRepository.Find(predicate);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public List<Domain.SystemForm> FindByEntityId(Guid entityId)
        {
            List<Domain.SystemForm> result = _cacheService.GetVersionItems(entityId.ToString(), () =>
            {
                return _systemFormRepository.Query(f => f.EntityId == entityId)?.ToList();
            });
            if (result != null)
            {
                WrapLocalizedLabel(result);
            }
            return result;
        }

        public List<Domain.SystemForm> FindByEntityName(string entityName)
        {
            List<Domain.SystemForm> result = _cacheService.GetVersionItems(entityName, () =>
             {
                 return _systemFormRepository.Query(f => f.EntityName == entityName)?.ToList();
             });
            if (result != null)
            {
                WrapLocalizedLabel(result);
            }
            return result;
        }

        public List<Domain.SystemForm> Query(Func<QueryDescriptor<Domain.SystemForm>, QueryDescriptor<Domain.SystemForm>> container)
        {
            QueryDescriptor<Domain.SystemForm> q = container(QueryDescriptorBuilder.Build<Domain.SystemForm>());
            var datas = _systemFormRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public PagedList<Domain.SystemForm> QueryPaged(Func<QueryDescriptor<Domain.SystemForm>, QueryDescriptor<Domain.SystemForm>> container)
        {
            QueryDescriptor<Domain.SystemForm> q = container(QueryDescriptorBuilder.Build<Domain.SystemForm>());
            var datas = _systemFormRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<Domain.SystemForm> QueryPaged(Func<QueryDescriptor<Domain.SystemForm>, QueryDescriptor<Domain.SystemForm>> container, Guid solutionId, bool existInSolution, FormType formType)
        {
            QueryDescriptor<Domain.SystemForm> q = container(QueryDescriptorBuilder.Build<Domain.SystemForm>());
            var datas = _systemFormRepository.QueryPaged(q, ModuleCollection.GetIdentity(formType == FormType.Dashboard ? DashBoardDefaults.ModuleName : FormDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.SystemForm> QueryAuthorized(Func<QueryDescriptor<Domain.SystemForm>, QueryDescriptor<Domain.SystemForm>> container, FormType formType)
        {
            container += (QueryDescriptor<Domain.SystemForm> x) => { x.Where(f => f.FormType == (int)formType); return x; };
            QueryDescriptor<Domain.SystemForm> q = container(QueryDescriptorBuilder.Build<Domain.SystemForm>());
            var datas = _systemFormRepository.Query(q)?.ToList();
            if (!_currentUser.IsSuperAdmin && datas.NotEmpty())
            {
                var formIds = datas.Where(x => x.AuthorizationEnabled).Select(x => x.SystemFormId).ToArray();
                if (formIds.NotEmpty())
                {
                    var authorizedItems = _roleObjectAccessService.Authorized(formType == FormType.Dashboard ? DashBoardDefaults.ModuleName : FormDefaults.ModuleName, formIds);
                    datas.RemoveAll(x => x.AuthorizationEnabled && !authorizedItems.Contains(x.SystemFormId));
                }
            }

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<Domain.SystemForm> FindAll()
        {
            var entities = _cacheService.GetVersionItems("all", () =>
             {
                 return PreCacheAll();
             });
            if (entities != null)
            {
                WrapLocalizedLabel(entities);
            }
            return entities;
        }

        private List<Domain.SystemForm> PreCacheAll()
        {
            return _systemFormRepository.FindAll()?.ToList();
        }

        #region dependency

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.Name } : null;
        }

        public int ComponentType => ModuleCollection.GetIdentity(FormDefaults.ModuleName);

        #endregion dependency

        private void WrapLocalizedLabel(IEnumerable<Domain.SystemForm> datas)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            if (datas.NotEmpty())
            {
                var ids = datas.Select(f => f.SystemFormId);
                var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(ids)));
                foreach (var d in datas)
                {
                    d.Name = _localizedLabelService.GetLabelText(labels, d.SystemFormId, "LocalizedName", d.Name);
                    d.Description = _localizedLabelService.GetLabelText(labels, d.SystemFormId, "Description", d.Description);
                }
            }
            */
        }

        private void WrapLocalizedLabel(Domain.SystemForm entity)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == entity.SystemFormId));
            entity.Name = _localizedLabelService.GetLabelText(labels, entity.SystemFormId, "LocalizedName", entity.Name);
            entity.Description = _localizedLabelService.GetLabelText(labels, entity.SystemFormId, "Description", entity.Description);
            */
        }
    }
}
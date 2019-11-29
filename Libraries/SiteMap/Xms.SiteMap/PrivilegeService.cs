using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Data.Provider;
using Xms.Event.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.SiteMap.Data;
using Xms.SiteMap.Domain;

namespace Xms.SiteMap
{
    /// <summary>
    /// 菜单服务
    /// </summary>
    public class PrivilegeService : IPrivilegeService
    {
        private readonly IPrivilegeRepository _privilegeRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IEventPublisher _eventPublisher;
        private readonly Caching.CacheManager<Privilege> _cacheService;
        private readonly IAppContext _appContext;

        public PrivilegeService(IAppContext appContext
            , IPrivilegeRepository privilegeRepository
            , ILocalizedLabelService localizedLabelService
            , IEventPublisher eventPublisher)
        {
            _appContext = appContext;
            _privilegeRepository = privilegeRepository;
            _localizedLabelService = localizedLabelService;
            _eventPublisher = eventPublisher;
            _cacheService = new Caching.CacheManager<Privilege>(_appContext.OrganizationUniqueName + ":privileges", BuildKey, _appContext.PlatformSettings.CacheEnabled, PreCacheAll);
        }

        /// <summary>
        /// 是否存在
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        public bool IsExists(Privilege entity)
        {
            if (entity.SystemName.IsNotEmpty() && entity.ClassName.IsNotEmpty() && entity.MethodName.IsNotEmpty())
            {
                var isExists = this.Find(n => n.Where(f => f.ClassName == entity.ClassName && f.MethodName == entity.MethodName && f.PrivilegeId != entity.PrivilegeId)) != null;
                if (isExists)
                {
                    return true;
                }
            }
            return false;
        }

        public bool Create(Privilege entity)
        {
            //判断重复
            if (IsExists(entity))
            {
                return false;
            }

            if (!entity.ParentPrivilegeId.Equals(Guid.Empty))
            {
                var parent = FindById(entity.ParentPrivilegeId);
                if (parent != null)
                {
                    entity.Level = parent.Level + 1;
                }
            }
            if (entity.Level <= 0)
            {
                entity.Level = 1;
            }
            var flag = _privilegeRepository.Create(entity);
            if (flag)
            {
                //add to cache
                _cacheService.SetEntity(entity);
                //本地化标签
                //_localizedLabelService.Create(SolutionService.DefaultSolutionId, entity.DisplayName.IfEmpty(""), LabelTypeCodeEnum.Privilege, "DisplayName", entity.PrivilegeId, this._appContext.Org.LanguageId);
            }
            return flag;
        }

        public bool Update(Privilege entity)
        {
            //判断重复
            if (IsExists(entity))
            {
                return false;
            }
            var original = _privilegeRepository.FindById(entity.PrivilegeId);
            var flag = _privilegeRepository.Update(entity);
            if (flag)
            {
                //localization
                _localizedLabelService.Update(entity.DisplayName.IfEmpty(""), "DisplayName", entity.PrivilegeId, this._appContext.BaseLanguage);
                //assigning roles
                if (original.AuthorizationEnabled || !entity.AuthorizationEnabled)
                {
                    _eventPublisher.Publish(new AuthorizationStateChangedEvent
                    {
                        ObjectId = new List<Guid> { entity.PrivilegeId }
                        ,
                        State = false
                        ,
                        ResourceName = SiteMapDefaults.ModuleName
                    });
                }

                //add to cache
                _cacheService.SetEntity(entity);
            }
            return flag;
        }

        public bool UpdateAuthorization(bool isAuthorization, params Guid[] id)
        {
            var context = UpdateContextBuilder.Build<Domain.Privilege>();
            context.Set(f => f.AuthorizationEnabled, isAuthorization);
            context.Where(f => f.PrivilegeId.In(id));
            var result = true;
            using (UnitOfWork.Build(_privilegeRepository.DbContext))
            {
                result = _privilegeRepository.Update(context);
                _eventPublisher.Publish(new AuthorizationStateChangedEvent
                {
                    ObjectId = id.ToList()
                    ,
                    State = isAuthorization
                    ,
                    ResourceName = SiteMapDefaults.ModuleName
                });
                //set to cache
                var items = _privilegeRepository.Query(f => f.PrivilegeId.In(id)).ToList();
                foreach (var item in items)
                {
                    _cacheService.SetEntity(item);
                }
            }
            return result;
        }

        public Privilege FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("privilegeid", id.ToString());
            var entity = _cacheService.Get(dic, () =>
            {
                return _privilegeRepository.FindById(id);
            });

            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }

            return entity;
        }

        public Privilege Find(string systemName, string className, string methodName)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("systemName", systemName);
            dic.Add("className", className);
            dic.Add("MethodName", methodName);
            var entity = _cacheService.Get(dic, () =>
            {
                return this.Find(n => n.Where(f => f.SystemName == systemName && f.ClassName == className && f.MethodName == methodName));
            });
            return entity;
        }

        public Privilege Find(string url)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("url", url.UrlEncode());
            var entity = _cacheService.Get(dic, () =>
            {
                return this.Find(n => n.Where(f => f.Url == url));
            });
            return entity;
        }

        public bool DeleteById(Guid id)
        {
            var entity = _privilegeRepository.FindById(id);
            var flag = _privilegeRepository.DeleteById(id);
            if (flag)
            {
                //localization
                _localizedLabelService.DeleteByObject(id);
                //remove from cache
                _cacheService.RemoveEntity(entity);
            }
            return flag;
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            var flag = true;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;
        }

        public PagedList<Privilege> QueryPaged(Func<QueryDescriptor<Privilege>, QueryDescriptor<Privilege>> container)
        {
            QueryDescriptor<Privilege> q = container(QueryDescriptorBuilder.Build<Privilege>());

            var datas = _privilegeRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Privilege> Query(Func<QueryDescriptor<Privilege>, QueryDescriptor<Privilege>> container)
        {
            QueryDescriptor<Privilege> q = container(QueryDescriptorBuilder.Build<Privilege>());
            var datas = _privilegeRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<Privilege> FindAll()
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

        private List<Privilege> PreCacheAll()
        {
            return _privilegeRepository.FindAll()?.ToList();
        }

        public List<Privilege> AllPrivileges
        {
            get
            {
                List<Privilege> entities = _cacheService.GetVersionItems("allOrder", () =>
                 {
                     return this.Query(n => n.Sort(s => s.SortDescending(f => f.DisplayOrder)));
                 });
                return entities;
            }
        }

        public Privilege Find(Func<QueryDescriptor<Privilege>, QueryDescriptor<Privilege>> container)
        {
            QueryDescriptor<Privilege> q = container(QueryDescriptorBuilder.Build<Privilege>());
            return _privilegeRepository.Find(q);
        }

        public int Move(Guid moveid, Guid targetid, Guid parentid, string position)
        {
            int result = _privilegeRepository.MoveNode(moveid, targetid, parentid, position);
            return result;
        }

        private void WrapLocalizedLabel(IEnumerable<Privilege> entities)
        {
            //if (entities.NotEmpty())
            //{
            //    var ids = entities.Select(f => f.PrivilegeId);
            //    var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(ids)));
            //    foreach (var d in entities)
            //    {
            //        d.DisplayName = _localizedLabelService.GetLabelText(labels, d.PrivilegeId, "DisplayName", d.DisplayName);
            //    }
            //}
        }

        private void WrapLocalizedLabel(Privilege entity)
        {
            //var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == entity.PrivilegeId));
            //entity.DisplayName = _localizedLabelService.GetLabelText(labels, entity.PrivilegeId, "DisplayName");
        }

        private string BuildKey(Privilege entity)
        {
            return entity.PrivilegeId + "/" + entity.SystemName + "/" + entity.ClassName + "/" + entity.MethodName + "/" + entity.Url.UrlEncode() + "/";
        }
    }
}
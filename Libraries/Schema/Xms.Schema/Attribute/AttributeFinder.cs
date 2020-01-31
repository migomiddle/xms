using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Dependency.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;

namespace Xms.Schema.Attribute
{
    /// <summary>
    /// 字段查询服务
    /// </summary>
    public class AttributeFinder : IAttributeFinder, IDependentLookup<Domain.Attribute>
    {
        private readonly IAttributeRepository _attributeRepository;
        private readonly Caching.CacheManager<Domain.Attribute> _cacheService;
        private readonly IAppContext _appContext;

        public AttributeFinder(IAppContext appContext
            , IAttributeRepository attributeRepository
            )
        {
            _appContext = appContext;
            _attributeRepository = attributeRepository;
            _cacheService = new Caching.CacheManager<Domain.Attribute>(_appContext.OrganizationUniqueName + ":attributes", _appContext.PlatformSettings.CacheEnabled);
        }

        public Domain.Attribute FindById(Guid id)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("attributeid", id.ToString());

            var entity = _cacheService.Get(dic, () =>
            {
                return _attributeRepository.FindById(id);
            });

            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public Domain.Attribute Find(Guid entityId, string name)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("entityid", entityId.ToString());
            dic.Add("name", name);

            var entity = _cacheService.Get(dic, () =>
            {
                return _attributeRepository.Find(n => n.EntityId == entityId && n.Name == name);
            });
            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public Domain.Attribute Find(string entityName, string name)
        {
            var dic = new Dictionary<string, string>();
            dic.Add("entityname", entityName);
            dic.Add("name", name);

            Domain.Attribute entity = _cacheService.Get(dic, () =>
             {
                 return _attributeRepository.Find(n => n.EntityName == entityName && n.Name == name);
             });
            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public List<Domain.Attribute> FindByName(Guid entityId, params string[] name)
        {
            var result = this.FindByEntityId(entityId)?.Where(x => name.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))?.ToList();
            if (result.NotEmpty())
            {
                WrapLocalizedLabel(result);
            }
            return result?.ToList();
        }

        public List<Domain.Attribute> FindByName(string entityName, params string[] name)
        {
            var result = this.FindByEntityName(entityName)?.Where(x => name.Contains(x.Name, StringComparer.InvariantCultureIgnoreCase))?.ToList();
            if (result.NotEmpty())
            {
                WrapLocalizedLabel(result);
            }
            return result?.ToList();
        }

        public List<Domain.Attribute> FindByEntityId(Guid entityId)
        {
            string sIndex = string.Join("/", entityId.ToString());
            List<Domain.Attribute> result = _cacheService.GetVersionItems(sIndex, () =>
             {
                 return _attributeRepository.Query(n => n.EntityId == entityId)?.ToList();
             });

            if (result.NotEmpty())
            {
                WrapLocalizedLabel(result);
            }
            return result;
        }

        public List<Domain.Attribute> FindByEntityName(string entityName)
        {
            string sIndex = string.Join("/", entityName);
            List<Domain.Attribute> result = _cacheService.GetVersionItems(sIndex, () =>
             {
                 return _attributeRepository.Query(n => n.EntityName == entityName)?.ToList();
             });
            if (result.NotEmpty())
            {
                WrapLocalizedLabel(result);
            }
            return result;
        }

        public Domain.Attribute Find(Expression<Func<Domain.Attribute, bool>> predicate)
        {
            var attr = _attributeRepository.Find(predicate);
            if (attr != null)
            {
                WrapLocalizedLabel(attr);
            }
            return attr;
        }

        public PagedList<Domain.Attribute> QueryPaged(Func<QueryDescriptor<Domain.Attribute>, QueryDescriptor<Domain.Attribute>> container)
        {
            QueryDescriptor<Domain.Attribute> q = container(QueryDescriptorBuilder.Build<Domain.Attribute>());

            var datas = _attributeRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.Attribute> Query(Func<QueryDescriptor<Domain.Attribute>, QueryDescriptor<Domain.Attribute>> container)
        {
            QueryDescriptor<Domain.Attribute> q = container(QueryDescriptorBuilder.Build<Domain.Attribute>());
            var datas = _attributeRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<Domain.Attribute> FindAll()
        {
            var entities = _cacheService.GetVersionItems("all", () =>
             {
                 return _attributeRepository.FindAll()?.ToList();
             });
            if (entities != null)
            {
                WrapLocalizedLabel(entities);
            }
            return entities;
        }

        public bool IsSysAttribute(string name)
        {
            string[] sysAttrs = new string[] { "Name", "VersionNumber", "CreatedOn", "CreatedBy", "ModifiedOn", "ModifiedBy", "StateCode", "StatusCode", "OwnerId", "OwnerIdType", "OwningBusinessUnit", "OrganizationId", "WorkFlowId", "ProcessState", "stageid" };
            return sysAttrs.Contains(name.ToLower(), StringComparer.InvariantCultureIgnoreCase);
        }

        public bool IsExists(Guid entityId, string name)
        {
            var attribute = Find(entityId, name);

            return attribute != null;
        }

        #region dependency

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.LocalizedName + "-" + result.Name } : null;
        }

        public int ComponentType => ModuleCollection.GetIdentity(AttributeDefaults.ModuleName);

        #endregion dependency

        #region 多语言标签

        private void WrapLocalizedLabel(IList<Domain.Attribute> attributes)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            if (attributes.NotEmpty())
            {
                var attrid = attributes.Select(f => f.AttributeId);
                if (attrid.Count() > 1000)
                {
                    int count = attrid.Count();
                    while (count > 1000)
                    {
                        var tmpAttrid = attrid.Take(1000);
                        var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(tmpAttrid)));
                        if (labels.NotEmpty())
                        {
                            foreach (var attr in attributes)
                            {
                                attr.LocalizedName = _localizedLabelService.GetLabelText(labels, attr.AttributeId, "LocalizedName", attr.LocalizedName);
                                attr.Description = _localizedLabelService.GetLabelText(labels, attr.AttributeId, "Description", attr.Description);
                            }
                        }
                        count -= 1000;
                    }
                }
                else
                {
                    var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(attrid)));
                    if (labels.NotEmpty())
                    {
                        foreach (var attr in attributes)
                        {
                            attr.LocalizedName = _localizedLabelService.GetLabelText(labels, attr.AttributeId, "LocalizedName", attr.LocalizedName);
                            attr.Description = _localizedLabelService.GetLabelText(labels, attr.AttributeId, "Description", attr.Description);
                        }
                    }
                }
            }*/
        }

        private void WrapLocalizedLabel(Domain.Attribute attr)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == attr.AttributeId));
            attr.LocalizedName = _localizedLabelService.GetLabelText(labels, attr.AttributeId, "LocalizedName", attr.LocalizedName);
            attr.Description = _localizedLabelService.GetLabelText(labels, attr.AttributeId, "Description", attr.Description);
            */
        }

        #endregion 多语言标签
    }
}
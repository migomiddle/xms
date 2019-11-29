using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Dependency.Abstractions;
using Xms.Module.Core;
using Xms.RibbonButton.Abstractions;
using Xms.RibbonButton.Data;

namespace Xms.RibbonButton
{
    /// <summary>
    /// 按钮查询服务
    /// </summary>
    public class RibbonButtonFinder : IRibbonButtonFinder, IDependentLookup<Domain.RibbonButton>
    {
        private readonly IRibbonButtonRepository _ribbonButtonRepository;
        //private readonly ILocalizedLabelService _localizedLabelService;
        //private readonly Caching.CacheManager<Domain.RibbonButton> _cacheService;

        public RibbonButtonFinder(//IAppContext appContext
            IRibbonButtonRepository ribbonButtonRepository
            //, ILocalizedLabelService localizedLabelService
            )
        {
            _ribbonButtonRepository = ribbonButtonRepository;
            //_localizedLabelService = localizedLabelService;
            //_cacheService = new Caching.CacheManager<Domain.RibbonButton>(appContext.OrganizationUniqueName + ":ribbonbuttons", RibbonButtonCache.BuildKey);
        }

        public Domain.RibbonButton FindById(Guid id)
        {
            var data = _ribbonButtonRepository.FindById(id);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public Domain.RibbonButton Find(Expression<Func<Domain.RibbonButton, bool>> predicate)
        {
            var data = _ribbonButtonRepository.Find(predicate);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public PagedList<Domain.RibbonButton> QueryPaged(Func<QueryDescriptor<Domain.RibbonButton>, QueryDescriptor<Domain.RibbonButton>> container)
        {
            QueryDescriptor<Domain.RibbonButton> q = container(QueryDescriptorBuilder.Build<Domain.RibbonButton>());
            var datas = _ribbonButtonRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<Domain.RibbonButton> QueryPaged(Func<QueryDescriptor<Domain.RibbonButton>, QueryDescriptor<Domain.RibbonButton>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<Domain.RibbonButton> q = container(QueryDescriptorBuilder.Build<Domain.RibbonButton>());
            var datas = _ribbonButtonRepository.QueryPaged(q, ModuleCollection.GetIdentity(RibbonButtonDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.RibbonButton> Query(Func<QueryDescriptor<Domain.RibbonButton>, QueryDescriptor<Domain.RibbonButton>> container)
        {
            QueryDescriptor<Domain.RibbonButton> q = container(QueryDescriptorBuilder.Build<Domain.RibbonButton>());
            var datas = _ribbonButtonRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<Domain.RibbonButton> FindAll()
        {
            //var entities = _cacheService.GetItems(() =>
            //{
            return PreCacheAll();
            //});
            //if (entities != null)
            //{
            //    WrapLocalizedLabel(entities);
            //}
            //return entities;
        }

        public List<Domain.RibbonButton> Find(Guid entityId, RibbonButtonArea? area)
        {
            if (area.HasValue)
            {
                return _ribbonButtonRepository.Query(x => x.EntityId == entityId && x.StateCode == Core.RecordState.Enabled && x.ShowArea == area.Value);
            }
            return _ribbonButtonRepository.Query(x => x.EntityId == entityId && x.StateCode == Core.RecordState.Enabled);
        }

        private List<Domain.RibbonButton> PreCacheAll()
        {
            return _ribbonButtonRepository.FindAll()?.ToList();
        }

        #region dependency

        public DependentDescriptor GetDependent(Guid dependentId)
        {
            var result = FindById(dependentId);
            return result != null ? new DependentDescriptor() { Name = result.Label } : null;
        }

        public int ComponentType => ModuleCollection.GetIdentity(RibbonButtonDefaults.ModuleName);

        #endregion dependency

        private void WrapLocalizedLabel(IEnumerable<Domain.RibbonButton> datas)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            if (datas.NotEmpty())
            {
                var ids = datas.Select(f => f.RibbonButtonId);
                var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(ids)));
                foreach (var d in datas)
                {
                    d.Label = _localizedLabelService.GetLabelText(labels, d.RibbonButtonId, "LocalizedName", d.Label);
                    d.Description = _localizedLabelService.GetLabelText(labels, d.RibbonButtonId, "Description", d.Description);
                }
            }*/
        }

        private void WrapLocalizedLabel(Domain.RibbonButton entity)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == entity.RibbonButtonId));
            entity.Label = _localizedLabelService.GetLabelText(labels, entity.RibbonButtonId, "LocalizedName", entity.Label);
            entity.Description = _localizedLabelService.GetLabelText(labels, entity.RibbonButtonId, "Description", entity.Description);
            */
        }
    }
}
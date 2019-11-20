using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Localization;
using Xms.Schema.Data;

namespace Xms.Schema.StringMap
{
    /// <summary>
    /// 字段选项查询服务
    /// </summary>
    public class StringMapFinder : IStringMapFinder
    {
        private readonly IStringMapRepository _stringMapRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IAppContext _appContext;

        public StringMapFinder(IAppContext appContext
            , IStringMapRepository stringMapRepository
            , ILocalizedLabelService localizedLabelService)
        {
            _appContext = appContext;
            _stringMapRepository = stringMapRepository;
            _localizedLabelService = localizedLabelService;
        }

        public Domain.StringMap FindById(Guid id)
        {
            var data = _stringMapRepository.FindById(id);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public Domain.StringMap Find(Expression<Func<Domain.StringMap, bool>> predicate)
        {
            var data = _stringMapRepository.Find(predicate);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public List<Domain.StringMap> FindByAttributeId(Guid attributeId)
        {
            var data = _stringMapRepository.Query(x => x.AttributeId == attributeId);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public PagedList<Domain.StringMap> QueryPaged(Func<QueryDescriptor<Domain.StringMap>, QueryDescriptor<Domain.StringMap>> container)
        {
            QueryDescriptor<Domain.StringMap> q = container(QueryDescriptorBuilder.Build<Domain.StringMap>());
            var datas = _stringMapRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.StringMap> Query(Func<QueryDescriptor<Domain.StringMap>, QueryDescriptor<Domain.StringMap>> container)
        {
            QueryDescriptor<Domain.StringMap> q = container(QueryDescriptorBuilder.Build<Domain.StringMap>());
            var datas = _stringMapRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public string GetOptionName(Guid attributeId, int value)
        {
            var o = _stringMapRepository.Find(f => f.AttributeId == attributeId && f.Value == value);
            if (o != null)
            {
                WrapLocalizedLabel(o);
                return o.Name;
            }
            return string.Empty;
        }

        private void WrapLocalizedLabel(IEnumerable<Domain.StringMap> datas)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            if (datas.NotEmpty())
            {
                var ids = datas.Select(f => f.StringMapId);
                var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(ids)));
                foreach (var d in datas)
                {
                    d.Name = _localizedLabelService.GetLabelText(labels, d.StringMapId, "LocalizedName", d.Name);
                }
            }*/
        }

        private void WrapLocalizedLabel(Domain.StringMap entity)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == entity.StringMapId));
            entity.Name = _localizedLabelService.GetLabelText(labels, entity.StringMapId, "LocalizedName", entity.Name);
            */
        }
    }
}
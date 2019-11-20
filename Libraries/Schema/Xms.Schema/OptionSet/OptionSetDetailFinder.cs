using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Localization;
using Xms.Schema.Data;

namespace Xms.Schema.OptionSet
{
    /// <summary>
    /// 选项集明细项目查询服务
    /// </summary>
    public class OptionSetDetailFinder : IOptionSetDetailFinder
    {
        private readonly IOptionSetDetailRepository _optionSetDetailRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IAppContext _appContext;

        public OptionSetDetailFinder(IAppContext appContext
            , IOptionSetDetailRepository optionSetDetailRepository
            , ILocalizedLabelService localizedLabelService)
        {
            _appContext = appContext;
            _optionSetDetailRepository = optionSetDetailRepository;
            _localizedLabelService = localizedLabelService;
        }

        public Domain.OptionSetDetail FindById(Guid id)
        {
            var entity = _optionSetDetailRepository.FindById(id);
            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public List<Domain.OptionSetDetail> FindByParentId(Guid optionSetId)
        {
            return _optionSetDetailRepository.Query(x => x.OptionSetId == optionSetId)?.OrderBy(o => o.DisplayOrder).ToList();
        }

        public Domain.OptionSetDetail Find(Expression<Func<Domain.OptionSetDetail, bool>> predicate)
        {
            var entity = _optionSetDetailRepository.Find(predicate);
            if (entity != null)
            {
                WrapLocalizedLabel(entity);
            }
            return entity;
        }

        public PagedList<Domain.OptionSetDetail> QueryPaged(Func<QueryDescriptor<Domain.OptionSetDetail>, QueryDescriptor<Domain.OptionSetDetail>> container)
        {
            QueryDescriptor<Domain.OptionSetDetail> q = container(QueryDescriptorBuilder.Build<Domain.OptionSetDetail>());
            var datas = _optionSetDetailRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<Domain.OptionSetDetail> Query(Func<QueryDescriptor<Domain.OptionSetDetail>, QueryDescriptor<Domain.OptionSetDetail>> container)
        {
            QueryDescriptor<Domain.OptionSetDetail> q = container(QueryDescriptorBuilder.Build<Domain.OptionSetDetail>());
            var datas = _optionSetDetailRepository.Query(q).ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public string GetOptionName(Guid optionSetId, int value)
        {
            var o = _optionSetDetailRepository.Find(f => f.OptionSetId == optionSetId && f.Value == value);
            if (o != null)
            {
                WrapLocalizedLabel(o);
            }
            return o != null ? o.Name : string.Empty;
        }

        private void WrapLocalizedLabel(IEnumerable<Domain.OptionSetDetail> datas)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            if (datas.NotEmpty())
            {
                var ids = datas.Select(f => f.OptionSetDetailId);
                var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId.In(ids)));
                foreach (var d in datas)
                {
                    d.Name = _localizedLabelService.GetLabelText(labels, d.OptionSetDetailId, "LocalizedName", d.Name);
                }
            }
            */
        }

        private void WrapLocalizedLabel(Domain.OptionSetDetail entity)
        {
            /* 先关闭多语言切换功能 2018-10-10* /
            /*
            var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == this.User.UserSettings.LanguageId && f.ObjectId == entity.OptionSetDetailId));
            entity.Name = _localizedLabelService.GetLabelText(labels, entity.OptionSetDetailId, "LocalizedName", entity.Name);
            */
        }
    }
}
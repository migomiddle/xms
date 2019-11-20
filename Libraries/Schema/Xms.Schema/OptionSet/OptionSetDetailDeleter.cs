using System;
using System.Linq;
using Xms.Context;
using Xms.Core.Data;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Schema.Data;

namespace Xms.Schema.OptionSet
{
    /// <summary>
    /// 选项集明细项目删除服务
    /// </summary>
    public class OptionSetDetailDeleter : IOptionSetDetailDeleter, ICascadeDelete<Domain.OptionSet>
    {
        private readonly IOptionSetDetailRepository _optionSetDetailRepository;
        private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IAppContext _appContext;

        public OptionSetDetailDeleter(IAppContext appContext
            , IOptionSetDetailRepository optionSetDetailRepository
            , ILocalizedLabelService localizedLabelService)
        {
            _appContext = appContext;
            _optionSetDetailRepository = optionSetDetailRepository;
            _localizedLabelService = localizedLabelService;
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的选项集</param>
        public void CascadeDelete(params Domain.OptionSet[] parent)
        {
            if (parent.IsEmpty())
            {
                return;
            }
            var ids = parent.Select(x => x.OptionSetId).ToArray();
            var deletedItems = _optionSetDetailRepository.Query(f => f.OptionSetId.In(ids));
            if (deletedItems.NotEmpty())
            {
                DeleteById(ids);
            }
        }

        public bool DeleteById(params Guid[] id)
        {
            Guard.NotEmpty(id, nameof(id));
            var result = false;
            using (UnitOfWork.Build(_optionSetDetailRepository.DbContext))
            {
                result = _optionSetDetailRepository.DeleteMany(id);
                //localization
                _localizedLabelService.DeleteByObject(id);
            }
            return result;
        }
    }
}
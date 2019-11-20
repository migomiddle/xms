using System.Collections.Generic;
using Xms.Data.Abstractions;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;
using Xms.Solution.Abstractions;

namespace Xms.Schema.OptionSet
{
    /// <summary>
    /// 选项集项目创建服务
    /// </summary>
    public class OptionSetDetailCreater : IOptionSetDetailCreater
    {
        private readonly IOptionSetDetailRepository _optionSetDetailRepository;
        private readonly ILocalizedLabelBatchBuilder _localizedLabelService;

        public OptionSetDetailCreater(IOptionSetDetailRepository optionSetDetailRepository
            , ILocalizedLabelBatchBuilder localizedLabelService)
        {
            _optionSetDetailRepository = optionSetDetailRepository;
            _localizedLabelService = localizedLabelService;
        }

        public bool Create(Domain.OptionSetDetail entity)
        {
            return CreateCore(entity);
        }

        public bool CreateMany(List<Domain.OptionSetDetail> entities)
        {
            return CreateCore(entities.ToArray());
        }

        private bool CreateCore(params Domain.OptionSetDetail[] entities)
        {
            Guard.NotEmpty(entities, nameof(entities));
            bool result = false;
            using (UnitOfWork.Build(_optionSetDetailRepository.DbContext))
            {
                result = _optionSetDetailRepository.CreateMany(entities);
                //本地化标签
                foreach (var entity in entities)
                {
                    _localizedLabelService.Append(SolutionDefaults.DefaultSolutionId, entity.Name.IfEmpty(""), OptionSetDefaults.ModuleName, "LocalizedName", entity.OptionSetDetailId);
                }
                _localizedLabelService.Save();
            }
            return result;
        }
    }
}
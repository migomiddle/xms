using System.Collections.Generic;
using Xms.Data.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Localization;
using Xms.Schema.Abstractions;
using Xms.Schema.Data;
using Xms.Solution.Abstractions;

namespace Xms.Schema.StringMap
{
    /// <summary>
    /// 字段选项创建服务
    /// </summary>
    public class StringMapCreater : IStringMapCreater
    {
        private readonly IStringMapRepository _stringMapRepository;
        private readonly ILocalizedLabelBatchBuilder _localizedLabelService;

        public StringMapCreater(IStringMapRepository stringMapRepository
            , ILocalizedLabelBatchBuilder localizedLabelService)
        {
            _stringMapRepository = stringMapRepository;
            _localizedLabelService = localizedLabelService;
        }

        public bool Create(Domain.StringMap entity)
        {
            var result = true;
            using (UnitOfWork.Build(_stringMapRepository.DbContext))
            {
                result = _stringMapRepository.Create(entity);
                //本地化标签
                _localizedLabelService.Append(SolutionDefaults.DefaultSolutionId, entity.Name.IfEmpty(""), StringMapDefaults.ModuleName, "LocalizedName", entity.StringMapId)
                    .Save();
            }
            return result;
        }

        public bool CreateMany(IEnumerable<Domain.StringMap> entities)
        {
            var result = true;
            using (UnitOfWork.Build(_stringMapRepository.DbContext))
            {
                result = _stringMapRepository.CreateMany(entities);
                //本地化标签
                foreach (var entity in entities)
                {
                    _localizedLabelService.Append(SolutionDefaults.DefaultSolutionId, entity.Name.IfEmpty(""), StringMapDefaults.ModuleName, "LocalizedName", entity.StringMapId);
                }
                _localizedLabelService.Save();
            }
            return result;
        }
    }
}
using System;
using Xms.Localization.Data;
using Xms.Solution.Abstractions;

namespace Xms.Localization
{
    /// <summary>
    /// 本地化标签导出XML
    /// </summary>
    public class LocalizedLabelExporter : ISolutionComponentExporter
    {
        private readonly ILocalizedLabelRepository _localizedLabelRepository;

        public LocalizedLabelExporter(ILocalizedLabelRepository localizedLabelRepository)
        {
            _localizedLabelRepository = localizedLabelRepository;
        }

        public string GetXml(Guid solutionId)
        {
            return _localizedLabelRepository.GetLocalizedLabelXml(solutionId);
        }
    }
}
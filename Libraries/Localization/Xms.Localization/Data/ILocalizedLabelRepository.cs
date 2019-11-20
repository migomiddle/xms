using System;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Localization.Abstractions;
using Xms.Localization.Domain;

namespace Xms.Localization.Data
{
    public interface ILocalizedLabelRepository : IRepository<LocalizedLabel>
    {
        bool DeleteByObject(Guid objectId, params string[] name);

        bool DeleteByObject(IEnumerable<Guid> objectId);

        List<dynamic> Export(Guid solutionId, LanguageCode baseLanguageId, IEnumerable<Language> languages);

        string GetLocalizedLabelXml(Guid solutionId);
    }
}
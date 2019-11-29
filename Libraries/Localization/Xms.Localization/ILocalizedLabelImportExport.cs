using System;
using Xms.Localization.Abstractions;

namespace Xms.Localization
{
    public interface ILocalizedLabelImportExport
    {
        string Export(Guid solutionId, LanguageCode baseLanguageId);

        bool Import(string file, Guid solutionId, LanguageCode baseLanguageId);
    }
}
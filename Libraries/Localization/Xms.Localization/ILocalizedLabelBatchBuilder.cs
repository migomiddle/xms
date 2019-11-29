using System;

namespace Xms.Localization
{
    public interface ILocalizedLabelBatchBuilder
    {
        ILocalizedLabelBatchBuilder Append(Guid solutionId, string label, string labelTypeName, string columnName, Guid objectId);

        void Clear();

        bool Save();
    }
}
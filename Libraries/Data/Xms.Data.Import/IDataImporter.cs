using System;
using Xms.Data.Import.Domain;

namespace Xms.Data.Import
{
    public interface IDataImporter
    {
        ImportFile Import(Guid importFileId);

        ImportFile RetryFailures(Guid importFileId);
    }
}
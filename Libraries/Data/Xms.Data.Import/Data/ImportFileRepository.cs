using Xms.Core.Data;
using Xms.Data.Import.Domain;

namespace Xms.Data.Import.Data
{
    /// <summary>
    /// 导入文件仓储
    /// </summary>
    public class ImportFileRepository : DefaultRepository<ImportFile>, IImportFileRepository
    {
        public ImportFileRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
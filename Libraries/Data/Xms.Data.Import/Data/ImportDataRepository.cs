using Xms.Core.Data;
using Xms.Data.Import.Domain;

namespace Xms.Data.Import.Data
{
    /// <summary>
    /// 导入数据仓储
    /// </summary>
    public class ImportDataRepository : DefaultRepository<ImportData>, IImportDataRepository
    {
        public ImportDataRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
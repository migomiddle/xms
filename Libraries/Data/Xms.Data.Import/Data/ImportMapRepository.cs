using Xms.Core.Data;
using Xms.Data.Import.Domain;

namespace Xms.Data.Import.Data
{
    /// <summary>
    /// 导入字段映射仓储
    /// </summary>
    public class ImportMapRepository : DefaultRepository<ImportMap>, IImportMapRepository
    {
        public ImportMapRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
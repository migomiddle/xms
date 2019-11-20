using Xms.Core.Data;
using Xms.Configuration.Domain;
using Xms.Data;

namespace Xms.Configuration.Data
{
    public class SettingRepository : DefaultRepository<Setting>, ISettingRepository
    {
        public SettingRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
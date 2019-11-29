using Xms.Core.Data;
using Xms.Data;
using Xms.Localization.Domain;

namespace Xms.Localization.Data
{
    public class LocalizedTextRepository : DefaultRepository<LocalizedResource>, ILocalizedTextRepository
    {
        public LocalizedTextRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
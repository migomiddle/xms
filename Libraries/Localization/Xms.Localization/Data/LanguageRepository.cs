using Xms.Core.Data;
using Xms.Data;
using Xms.Localization.Domain;

namespace Xms.Localization.Data
{
    /// <summary>
    /// 语言仓储
    /// </summary>
    public class LanguageRepository : DefaultRepository<Language>, ILanguageRepository
    {
        public LanguageRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
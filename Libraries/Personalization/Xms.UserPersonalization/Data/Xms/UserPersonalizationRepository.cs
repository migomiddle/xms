using Xms.Core.Data;
using Xms.Data;

namespace Xms.UserPersonalization.Data.Xms
{
    public class UserPersonalizationRepository : DefaultRepository<Domain.UserCustomization>, IUserPersonalizationRepository
    {
        public UserPersonalizationRepository(IDbContext dbContext) : base(dbContext)
        {
        }
    }
}
using System;
using System.Collections.Generic;

namespace Xms.UserPersonalization.Data
{
    public interface IUserPersonalizationRepository
    {
        List<Domain.UserPersonalization> Get(Guid ownerId);

        Domain.UserPersonalization GetByName(Guid ownerId, string name);

        bool Set(Domain.UserPersonalization userPersonalization);

        bool Delete(Guid ownerId);

        bool Delete(Guid ownerId, string name);

        bool DeleteById(Guid id);
    }
}
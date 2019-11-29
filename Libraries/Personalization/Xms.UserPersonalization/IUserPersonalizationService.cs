using System;
using System.Collections.Generic;

namespace Xms.UserPersonalization
{
    public interface IUserPersonalizationService
    {
        bool Set(Domain.UserPersonalization userPersonalization);

        List<Domain.UserPersonalization> Get(Guid ownerId);

        Domain.UserPersonalization GetByName(Guid ownerId, string name);

        bool Delete(Guid ownerId);

        bool Delete(Guid ownerId, string name);

        bool DeleteById(Guid id);
    }
}
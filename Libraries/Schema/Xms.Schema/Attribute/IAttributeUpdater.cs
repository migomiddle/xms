using System;
using System.Collections.Generic;

namespace Xms.Schema.Attribute
{
    public interface IAttributeUpdater
    {
        bool Update(Domain.Attribute entity);
        bool UpdateAuthorization(bool isAuthorization, params Guid[] id);
    }
}
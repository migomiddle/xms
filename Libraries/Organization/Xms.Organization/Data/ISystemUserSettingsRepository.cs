using System;
using Xms.Organization.Domain;

namespace Xms.Organization.Data
{
    public interface ISystemUserSettingsRepository
    {
        UserSettings FindById(Guid id);
    }
}
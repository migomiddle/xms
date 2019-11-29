using System.Collections.Generic;
using Xms.Configuration.Domain;

namespace Xms.Configuration
{
    public interface ISettingService
    {
        bool SaveMany(IList<Setting> entities);

        bool Save<T>(T setting, string nameSpace = "");

        bool Save(IDictionary<string, object> keyValues);
    }
}
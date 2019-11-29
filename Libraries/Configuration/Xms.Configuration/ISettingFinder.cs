using System.Collections.Generic;

namespace Xms.Configuration
{
    public interface ISettingFinder
    {
        T Get<T>(string nameSpace = "") where T : new();

        Dictionary<string, string> GetKeyValues(string nameSpace = "");
    }
}
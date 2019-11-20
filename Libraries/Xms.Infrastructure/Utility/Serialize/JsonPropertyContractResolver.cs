using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Xms.Infrastructure.Utility.Serialize
{
    /// <summary>
    /// Json分析器
    /// </summary>
    public class JsonPropertyContractResolver : DefaultContractResolver
    {
        private readonly IEnumerable<string> _excludes;
        private readonly bool _nameLower;

        public JsonPropertyContractResolver(IEnumerable<string> excludedProperties, bool nameLower)
        {
            _excludes = excludedProperties;
            _nameLower = nameLower;
        }

        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            if (_nameLower)
            {
                var results = base.CreateProperties(type, memberSerialization).ToList().FindAll(p => !_excludes.Contains(p.PropertyName, StringComparer.InvariantCultureIgnoreCase));
                results.ForEach((n) => { n.PropertyName = n.PropertyName.ToLower(); });
                return results;
            }
            else
            {
                return base.CreateProperties(type, memberSerialization).ToList().FindAll(p => !_excludes.Contains(p.PropertyName, StringComparer.InvariantCultureIgnoreCase));
            }
        }
    }
}
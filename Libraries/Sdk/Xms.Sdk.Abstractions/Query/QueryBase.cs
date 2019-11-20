using Xms.Localization.Abstractions;

namespace Xms.Sdk.Abstractions.Query
{
    /// <summary>
    /// 查询对象
    /// </summary>
    public class QueryBase
    {
        public QueryBase(string entityName, LanguageCode languageId)
        {
            EntityName = entityName;
            LanguageId = languageId;
        }

        public string EntityName { get; set; }

        public LanguageCode LanguageId { get; set; }
    }
}
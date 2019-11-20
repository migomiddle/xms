using PetaPoco;
using System;
using Xms.Localization.Abstractions;

namespace Xms.Localization.Domain
{
    [TableName("LocalizedResource")]
    [PrimaryKey("LocalizedResourceId", AutoIncrement = false)]
    public class LocalizedResource
    {
        public Guid LocalizedResourceId { get; set; } = Guid.NewGuid();
        public string Name { get; set; }
        public string Text { get; set; }
        public LanguageCode LanguageId { get; set; } = LanguageCode.CHS;
    }
}
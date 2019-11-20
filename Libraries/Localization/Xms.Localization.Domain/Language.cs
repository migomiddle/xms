using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Localization.Domain
{
    [TableName("Language")]
    [PrimaryKey("LanguageId", AutoIncrement = false)]
    public class Language
    {
        public Guid LanguageId { get; set; } = Guid.NewGuid();

        public string Name { get; set; }

        public string Code { get; set; }

        public int UniqueId { get; set; }
        public RecordState StateCode { get; set; }
    }
}
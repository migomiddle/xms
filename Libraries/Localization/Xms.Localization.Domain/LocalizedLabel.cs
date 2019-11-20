using PetaPoco;
using System;
using Xms.Localization.Abstractions;

namespace Xms.Localization.Domain
{
    [TableName("LocalizedLabel")]
    [PrimaryKey("LocalizedLabelId", AutoIncrement = false)]
    public class LocalizedLabel
    {
        public Guid LocalizedLabelId { get; set; } = Guid.NewGuid();

        public string Label { get; set; }

        public LanguageCode LanguageId { get; set; }

        public Guid ObjectId { get; set; }

        public string ObjectColumnName { get; set; }

        public int LabelTypeCode { get; set; }

        public Guid SolutionId { get; set; }

        public int ComponentState { get; set; }
    }
}
using PetaPoco;
using System;
using Xms.Core;
using Xms.RibbonButton.Abstractions;

namespace Xms.RibbonButton.Domain
{
    [TableName("RibbonButton")]
    [PrimaryKey("RibbonButtonId", AutoIncrement = false)]
    public class RibbonButton
    {
        public Guid RibbonButtonId { get; set; } = Guid.NewGuid();

        public string Label { get; set; }

        public string CssClass { get; set; }
        public string Icon { get; set; }

        public int DisplayOrder { get; set; }
        public string JsLibrary { get; set; }

        public string JsAction { get; set; }

        public Guid EntityId { get; set; }

        public RibbonButtonArea ShowArea { get; set; }

        public RecordState StateCode { get; set; }
        public string Description { get; set; }

        public Guid CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public Guid SolutionId { get; set; }
        public int ComponentState { get; set; }

        public string CommandRules { get; set; }

        public bool ShowLabel { get; set; }

        public bool AuthorizationEnabled { get; set; }

        [Ignore]
        public bool IsEnabled { get; set; } = true;

        [Ignore]
        public bool IsVisibled { get; set; } = true;
    }
}
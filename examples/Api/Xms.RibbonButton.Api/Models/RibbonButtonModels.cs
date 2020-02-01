using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xms.Core;
using Xms.RibbonButton.Abstractions;
using Xms.Web.Framework.Paging;

namespace Xms.RibbonButton.Api.Models
{
    public class RibbonButtonModel : BasePaged<RibbonButton.Domain.RibbonButton>
    {
        public string Label { get; set; }

        public RibbonButtonArea? ShowArea { get; set; }
        public RecordState? StateCode { get; set; }

        public Guid EntityId { get; set; }
        public Schema.Domain.Entity Entity { get; set; }
        public Guid SolutionId { get; set; }

    }

    public class EditRibbonButtonModel
    {
        public Guid? RibbonButtonId { get; set; }

        public string Label { get; set; }
        public string CssClass { get; set; }

        public string Icon { get; set; }

        public int DisplayOrder { get; set; }
        public string JsLibrary { get; set; }

        public string JsAction { get; set; }

        public Guid EntityId { get; set; }
        public RecordState StateCode { get; set; }

        public RibbonButtonArea ShowArea { get; set; }
        public Guid SolutionId { get; set; }

        public string CommandRules { get; set; }
        public string WebResourceName { get; set; }
    }

    public class SetRecordStateModel
    {
        public Guid[] RecordId { get; set; }
        public bool IsEnabled { get; set; }
        public string Name { get; set; }
    }
}

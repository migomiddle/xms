using System;
using System.Collections.Generic;
using Xms.Core;
using Xms.RibbonButton.Abstractions;
using Xms.Web.Framework.Paging;
using Xms.Web.Models;

namespace Xms.Web.Customize.Models
{
    public class RibbonButtonModel : BasePaged<RibbonButton.Domain.RibbonButton>
    {
        public string Label { get; set; }

        public RibbonButtonArea? ShowArea { get; set; }
        public RecordState? StateCode { get; set; }

        public Guid EntityId { get; set; }
        public Schema.Domain.Entity Entity { get; set; }
        public Guid SolutionId { get; set; }
        public bool LoadData { get; set; }
    }

    public class CustomButtonsDialogModel : DialogModel
    {
        public Guid EntityId { get; set; }
        public Guid ObjectId { get; set; }
        public bool IsCustomButton { get; set; }
        public RibbonButtonArea? ButtonArea { get; set; }
        public List<Guid> CustomButtons { get; set; }
        public List<RibbonButton.Domain.RibbonButton> Buttons { get; set; }
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
        public bool ShowLabel { get; set; }
        public string WebResourceName { get; set; }
    }
}
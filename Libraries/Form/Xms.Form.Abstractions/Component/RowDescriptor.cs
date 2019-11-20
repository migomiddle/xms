using System.Collections.Generic;

namespace Xms.Form.Abstractions.Component
{
    public sealed class RowDescriptor
    {
        public bool IsVisible { get; set; } = true;
        public List<CellDescriptor> Cells { get; set; }
    }
}
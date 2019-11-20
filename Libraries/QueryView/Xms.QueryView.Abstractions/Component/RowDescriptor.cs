using System.Collections.Generic;

namespace Xms.QueryView.Abstractions.Component
{
    public sealed class RowDescriptor
    {
        public string Name { get; set; }
        public string Id { get; set; }

        private List<CellDescriptor> _cells;

        public List<CellDescriptor> Cells
        {
            get
            {
                if (_cells == null)
                {
                    _cells = new List<CellDescriptor>();
                }
                return _cells;
            }
            set { _cells = value; }
        }

        public void AddCell(CellDescriptor cell)
        {
            this.Cells.Add(cell);
        }
    }
}
using System.Collections.Generic;
using Xms.Core.Components.Platform;

namespace Xms.QueryView.Abstractions.Component
{
    public sealed class GridDescriptor
    {
        private List<QueryColumnSortInfo> _sortColumns;

        public List<QueryColumnSortInfo> SortColumns
        {
            get
            {
                if (_sortColumns == null)
                {
                    _sortColumns = new List<QueryColumnSortInfo>();
                }
                return _sortColumns;
            }
            set { _sortColumns = value; }
        }

        private List<RowDescriptor> _rows;

        public List<RowDescriptor> Rows
        {
            get
            {
                if (_rows == null)
                {
                    _rows = new List<RowDescriptor>();
                }
                return _rows;
            }
            set { _rows = value; }
        }

        public RowCommand[] RowCommand { get; set; }

        public List<string> ClientResources { get; set; }

        //public PageList<dynamic> DataSource { get; set; }

        public void AddRow(RowDescriptor row)
        {
            this.Rows.Add(row);
        }

        public void AddSort(QueryColumnSortInfo sort)
        {
            this.SortColumns.Add(sort);
        }
    }
}
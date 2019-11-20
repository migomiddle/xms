using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Xms.Infrastructure.Utility;

namespace Xms.Sdk.Abstractions.Query
{
    public sealed class ColumnSet
    {
        private bool _allColumns;
        private List<string> _columns;
        private Dictionary<string, string> _columnFormatting;

        [DataMember]
        public bool AllColumns
        {
            get
            {
                return this._allColumns;
            }
            set
            {
                this._allColumns = value;
            }
        }

        [DataMember]
        public List<string> Columns
        {
            get
            {
                if (this._columns == null)
                {
                    this._columns = new List<string>();
                }
                return this._columns;
            }
        }

        [DataMember]
        public Dictionary<string, string> ColumnFormatting
        {
            get
            {
                if (this._columnFormatting == null)
                {
                    this._columnFormatting = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);
                }
                return this._columnFormatting;
            }
        }

        public ColumnSet()
        {
        }

        public ColumnSet(bool allColumns)
        {
            this._allColumns = allColumns;
        }

        public ColumnSet(params string[] columns)
        {
            this._columns = new List<string>(columns);
        }

        public void AddColumns(params string[] columns)
        {
            if (columns.NotEmpty())
            {
                this.Columns.AddRange(columns);
            }
        }

        public void AddColumn(string column)
        {
            this.Columns.Add(column);
        }

        public void AddColumnFormatting(string key, string value)
        {
            if (!ColumnFormatting.ContainsKey(key))
            {
                this.ColumnFormatting.Add(key, value);
            }
        }
    }
}
namespace Xms.Core.Components.Platform
{
    public sealed class QueryColumnSortInfo
    {
        public string Name { get; set; } = string.Empty;

        public bool SortAscending { get; set; } = true;

        public QueryColumnSortInfo(string name, bool sortAscending)
        {
            this.Name = name;
            this.SortAscending = sortAscending;
        }

        public QueryColumnSortInfo()
        {
        }
    }
}
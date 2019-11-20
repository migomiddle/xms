using System.Collections.Generic;

namespace Xms.Core.Context
{
    public class PagedList<T>
    {
        public PagedList()
        {
        }

        public long CurrentPage { get; set; }
        public List<T> Items { get; set; }
        public long ItemsPerPage { get; set; }
        public long TotalItems { get; set; }
        public long TotalPages { get; set; }

        public static PagedList<T> Default
        {
            get
            {
                return new PagedList<T>()
                {
                    Items = new List<T>()
                };
            }
        }
    }
}
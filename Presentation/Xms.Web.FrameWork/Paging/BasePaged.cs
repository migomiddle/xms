using System.Collections.Generic;
using Xms.Core.Context;

namespace Xms.Web.Framework.Paging
{
    public class BasePaged<T> where T : class
    {
        /// <summary>
        /// 查询所有记录
        /// </summary>
        public bool GetAll { get; set; }

        /// <summary>
        /// 结果数
        /// </summary>
        public long TotalItems { get; set; }

        private long _totalpages;

        /// <summary>
        /// 总页数
        /// </summary>
        public long TotalPages
        {
            get
            {
                _totalpages = TotalItems / PageSize;
                if (TotalItems % PageSize > 0)
                    _totalpages++;
                return _totalpages;
            }
        }

        //public PagedList<T> PagedResult { get; set; }
        /// <summary>
        /// 结果集
        /// </summary>
        public IList<T> Items { get; set; }

        private List<SortDescriptor<T>> _sortingInfo;

        /// <summary>
        /// 排序信息
        /// </summary>
        public List<SortDescriptor<T>> SortingInfo
        {
            get
            {
                if (_sortingInfo == null)
                {
                    _sortingInfo = new List<SortDescriptor<T>>();
                }
                return _sortingInfo;
            }
            set
            {
                _sortingInfo = value;
            }
        }

        private int _page = 1;

        /// <summary>
        /// 当前页
        /// </summary>
        public int Page
        {
            get
            {
                return _page > 0 ? _page : 1;
            }
            set
            {
                _page = value;
            }
        }

        private int _pageSize = 10;

        /// <summary>
        /// 每页记录数
        /// </summary>
        public int PageSize
        {
            get
            {
                return _pageSize > 0 ? _pageSize : 10;
            }
            set
            {
                _pageSize = value;
                PageSizeBySeted = true;
            }
        }

        public bool PageSizeBySeted { private set; get; }

        private string _sortby = "CreatedOn";

        /// <summary>
        /// 排序字段名称
        /// </summary>
        public string SortBy
        {
            get
            {
                return _sortby;
            }
            set
            {
                IsSortBySeted = true;
                _sortby = value;
            }
        }

        public bool IsSortBySeted
        {
            private set;
            get;
        }

        /// <summary>
        /// 排序类型
        /// </summary>
        public int SortDirection { get; set; } = 1;
    }
}
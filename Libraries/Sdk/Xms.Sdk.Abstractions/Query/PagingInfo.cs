using System.Runtime.Serialization;

namespace Xms.Sdk.Abstractions.Query
{
    public sealed class PagingInfo
    {
        private int _pageNumber = 1;
        private int _count;
        private int _pageSize = 10;
        private string _pagingCookie;
        private bool _returnTotalRecordCount;

        [DataMember]
        public int PageNumber
        {
            get
            {
                return this._pageNumber;
            }
            set
            {
                this._pageNumber = value;
            }
        }

        [DataMember]
        public int PageSize
        {
            get
            {
                return this._pageSize;
            }
            set
            {
                this._pageSize = value;
            }
        }

        [DataMember]
        public int Count
        {
            get
            {
                return this._count;
            }
            set
            {
                this._count = value;
            }
        }

        [DataMember]
        public bool ReturnTotalRecordCount
        {
            get
            {
                return this._returnTotalRecordCount;
            }
            set
            {
                this._returnTotalRecordCount = value;
            }
        }

        [DataMember]
        public string PagingCookie
        {
            get
            {
                return this._pagingCookie;
            }
            set
            {
                this._pagingCookie = value;
            }
        }
    }
}
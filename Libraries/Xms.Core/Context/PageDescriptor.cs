namespace Xms.Core.Context
{
    public class PageDescriptor
    {
        public PageDescriptor()
        {
        }

        public PageDescriptor(int pageNumber, int pageSize)
        {
            this.PageSize = pageSize;
            this.PageNumber = pageNumber;
        }

        private int _pageSize = 10;

        public int PageSize
        {
            get
            {
                return _pageSize > 0 ? _pageSize : 10;
            }
            set
            {
                _pageSize = value;
            }
        }

        private int _pageNumber;

        public int PageNumber
        {
            get
            {
                return _pageNumber > 0 ? _pageNumber : 1;
            }
            set
            {
                _pageNumber = value;
            }
        }
    }
}
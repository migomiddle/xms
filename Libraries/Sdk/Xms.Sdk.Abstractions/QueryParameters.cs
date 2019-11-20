using System.Collections.Generic;

namespace Xms.Sdk.Abstractions
{
    public sealed class QueryParameters
    {
        public string QueryString { get; set; }

        private List<object> _args = new List<object>();

        public List<object> Args
        {
            get
            {
                return _args;
            }
            set { _args = value; }
        }
    }
}
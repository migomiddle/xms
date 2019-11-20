using System.Collections.Generic;
using Xms.Localization.Abstractions;

namespace Xms.Sdk.Abstractions.Query
{
    public sealed class QueryByAttribute : QueryBase
    {
        //private string _entityName;
        private ColumnSet _columnSet;

        private PagingInfo _pageInfo;
        private List<OrderExpression> _orders;
        private List<string> _attributes;
        private List<object> _attributeValues;

        public QueryByAttribute() : this(null, LanguageCode.CHS)
        {
        }

        public QueryByAttribute(string entityName, LanguageCode languageId) : base(entityName, languageId)
        {
            //this.EntityName = entityName;
        }

        //public string EntityName
        //{
        //    get
        //    {
        //        return this._entityName;
        //    }
        //    set
        //    {
        //        this._entityName = value;
        //    }
        //}

        public ColumnSet ColumnSet
        {
            get
            {
                if (this._columnSet == null)
                {
                    this._columnSet = new ColumnSet();
                }
                return this._columnSet;
            }
            set
            {
                this._columnSet = value;
            }
        }

        public PagingInfo PageInfo
        {
            get
            {
                return this._pageInfo;
            }
            set
            {
                this._pageInfo = value;
            }
        }

        public List<OrderExpression> Orders
        {
            get
            {
                if (this._orders == null)
                {
                    this._orders = new List<OrderExpression>();
                }
                return this._orders;
            }
            private set
            {
                this._orders = value;
            }
        }

        public List<string> Attributes
        {
            get
            {
                if (this._attributes == null)
                {
                    this._attributes = new List<string>();
                }
                return this._attributes;
            }
            set
            {
                this._attributes = value;
            }
        }

        public List<object> Values
        {
            get
            {
                if (this._attributeValues == null)
                {
                    this._attributeValues = new List<object>();
                }
                return this._attributeValues;
            }
            private set
            {
                this._attributeValues = value;
            }
        }

        public void AddOrder(string attributeName, OrderType orderType)
        {
            this.Orders.Add(new OrderExpression(attributeName, orderType));
        }
    }
}
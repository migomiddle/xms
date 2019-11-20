using System.Collections.Generic;
using Xms.Localization.Abstractions;

namespace Xms.Sdk.Abstractions.Query
{
    public class QueryExpression : QueryBase
    {
        private List<LinkEntity> _linkEntities;
        private List<OrderExpression> _orders;

        public int Top { get; set; }

        public bool Distinct { get; set; }

        public bool NoLock { get; set; } = true;

        public PagingInfo PageInfo { get; set; }

        public List<LinkEntity> LinkEntities
        {
            get
            {
                if (this._linkEntities == null)
                {
                    this._linkEntities = new List<LinkEntity>();
                }
                return this._linkEntities;
            }
            set
            {
                this._linkEntities = value;
            }
        }

        public FilterExpression Criteria { get; set; }

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

        public ColumnSet ColumnSet { get; set; }

        public QueryExpression() : this(null, LanguageCode.CHS)
        {
        }

        public QueryExpression(string entityName, LanguageCode languageId = LanguageCode.CHS) : base(entityName, languageId)
        {
            this.Criteria = new FilterExpression();
            this.PageInfo = new PagingInfo();
            this.ColumnSet = new ColumnSet();
        }

        public void AddColumns(params string[] columns)
        {
            this.ColumnSet.AddColumns(columns);
        }

        public void AddOrder(string attributeName, OrderType orderType)
        {
            this.Orders.Add(new OrderExpression(attributeName, orderType));
        }

        public LinkEntity AddLink(string linkToEntityName, string linkFromAttributeName, string linkToAttributeName)
        {
            return this.AddLink(linkToEntityName, linkFromAttributeName, linkToAttributeName, JoinOperator.Inner);
        }

        public LinkEntity AddLink(string linkToEntityName, string linkFromAttributeName, string linkToAttributeName, JoinOperator joinOperator)
        {
            LinkEntity linkEntity = new LinkEntity(this.EntityName, linkToEntityName, linkFromAttributeName, linkToAttributeName, joinOperator);
            this.LinkEntities.Add(linkEntity);
            return linkEntity;
        }
    }
}
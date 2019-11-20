using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Xms.Sdk.Abstractions.Query
{
    public sealed class LinkEntity
    {
        private string _linkFromEntityName;
        private string _linkFromAttributeName;
        private string _linkToEntityName;
        private string _linkToAttributeName;
        private JoinOperator _joinOperator;
        private FilterExpression _linkCriteria;
        private string _entityAlias;
        private string _fromEntityAlias;
        private ColumnSet _columns;
        private List<LinkEntity> _linkEntities;

        [DataMember]
        public string LinkFromAttributeName
        {
            get
            {
                return this._linkFromAttributeName;
            }
            set
            {
                this._linkFromAttributeName = value;
            }
        }

        [DataMember]
        public string LinkFromEntityName
        {
            get
            {
                return this._linkFromEntityName;
            }
            set
            {
                this._linkFromEntityName = value;
            }
        }

        [DataMember]
        public string LinkToEntityName
        {
            get
            {
                return this._linkToEntityName;
            }
            set
            {
                this._linkToEntityName = value;
            }
        }

        [DataMember]
        public string LinkToAttributeName
        {
            get
            {
                return this._linkToAttributeName;
            }
            set
            {
                this._linkToAttributeName = value;
            }
        }

        [DataMember]
        public JoinOperator JoinOperator
        {
            get
            {
                return this._joinOperator;
            }
            set
            {
                this._joinOperator = value;
            }
        }

        [DataMember]
        public FilterExpression LinkCriteria
        {
            get
            {
                return this._linkCriteria;
            }
            set
            {
                this._linkCriteria = value;
            }
        }

        [DataMember]
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
            private set
            {
                this._linkEntities = value;
            }
        }

        [DataMember]
        public ColumnSet Columns
        {
            get
            {
                if (this._columns == null)
                {
                    this._columns = new ColumnSet();
                }
                return this._columns;
            }
            set
            {
                this._columns = value;
            }
        }

        [DataMember]
        public string EntityAlias
        {
            get
            {
                return this._entityAlias;
            }
            set
            {
                this._entityAlias = value;
            }
        }

        [DataMember]
        public string FromEntityAlias
        {
            get
            {
                return this._fromEntityAlias;
            }
            set
            {
                this._fromEntityAlias = value;
            }
        }

        public LinkEntity() : this(null, null, null, null, JoinOperator.Inner)
        {
        }

        public LinkEntity(string linkFromEntityName, string linkToEntityName, string linkFromAttributeName, string linkToAttributeName, JoinOperator joinOperator)
        {
            this._entityAlias = "LE_" + Guid.NewGuid().ToString("N");
            this._linkFromEntityName = linkFromEntityName;
            this._linkToEntityName = linkToEntityName;
            this._linkFromAttributeName = linkFromAttributeName;
            this._linkToAttributeName = linkToAttributeName;
            this._joinOperator = joinOperator;
            this._columns = new ColumnSet();
            this._linkCriteria = new FilterExpression();
        }

        public LinkEntity AddLink(string linkToEntityName, string linkFromAttributeName, string linkToAttributeName)
        {
            return this.AddLink(linkToEntityName, linkFromAttributeName, linkToAttributeName, JoinOperator.Inner);
        }

        public LinkEntity AddLink(string linkToEntityName, string linkFromAttributeName, string linkToAttributeName, JoinOperator joinOperator)
        {
            LinkEntity linkEntity = new LinkEntity(this._linkFromEntityName, linkToEntityName, linkFromAttributeName, linkToAttributeName, joinOperator);
            this.LinkEntities.Add(linkEntity);
            return linkEntity;
        }
    }
}
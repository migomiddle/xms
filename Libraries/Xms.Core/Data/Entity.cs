using System;
using System.Collections.Generic;
using Xms.Infrastructure.Utility;

namespace Xms.Core.Data
{
    public class Entity : Dictionary<string, object>
    {
        public Entity() : base(StringComparer.OrdinalIgnoreCase)
        {
        }

        public Entity(string name, IDictionary<string, object> dictionary) : base(dictionary, StringComparer.OrdinalIgnoreCase)
        {
            this.Name = name;
        }

        public Entity(string name) : base(StringComparer.OrdinalIgnoreCase)
        {
            this.Name = name;
        }

        private Guid _id;

        public Guid Id
        {
            get
            {
                if (_id.Equals(Guid.Empty))
                {
                    _id = this.GetIdValue();
                }
                return _id;
            }
            set
            {
                _id = value;
            }
        }

        public string Name { get; set; }
        private string _idName;

        public string IdName
        {
            get { return _idName.IfEmpty(Name + "id"); }
            set { _idName = value; }
        }

        public Entity AddIfNotContain(string key, object value)
        {
            if (!this.ContainsKey(key))
            {
                this.Add(key, value);
            }
            else if (this[key] == null && value != null)
            {
                this[key] = value;
            }
            return this;
        }

        public Entity SetIdValue(Guid value, string idName = "")
        {
            if (idName.IsNotEmpty())
            {
                IdName = idName;
            }
            this[IdName] = value;
            Id = value;
            return this;
        }

        public Guid GetIdValue()
        {
            if (this.ContainsKey(IdName))
            {
                return Guid.Parse(this[IdName].ToString());
            }
            return Guid.Empty;
        }

        public Entity SetIdName(string idName)
        {
            IdName = idName;
            return this;
        }

        public Entity SetAttributeValue(string key, object value)
        {
            this[key] = value;
            return this;
        }
    }
}
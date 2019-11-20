using System;

namespace Xms.Sdk.Abstractions
{
    public class EntityReference
    {
        public string ReferencedEntityName { get; set; }
        public Guid ReferencedValue { get; set; }

        public EntityReference(string referencedEntityName, Guid referencedValue)
        {
            this.ReferencedEntityName = referencedEntityName;
            this.ReferencedValue = referencedValue;
        }
    }
}
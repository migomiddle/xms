using System;
using System.Collections.Generic;

namespace Xms.Sdk.Event.AggRoot
{
    public class AggregateRootMetaData
    {
        public Guid? SystemFormId { get; set; }
        public EntityAttributeMetadata MainMetadata { get; set; }
        public Dictionary<string, EntityAttributeMetadata> ListMetadatas { get; set; }
    }

    public class EntityAttributeMetadata
    {
        public Schema.Domain.Entity EntityMetadata { get; set; }
        public List<Schema.Domain.Attribute> AttributeMetadatas { get; set; }
    }
}
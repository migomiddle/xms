using System;
using System.Collections.Generic;

namespace Xms.Core.Data
{
    public class AggregateRoot : IAggregateRoot
    {
        public Entity MainEntity { get; set; }

        public Dictionary<Guid, List<dynamic>> Grids { get; set; }
        public List<RefEntity> ChildEntities { get; set; }

        public AggregateRoot()
        {
            Grids = new Dictionary<Guid, List<dynamic>>();
            ChildEntities = new List<RefEntity>();
        }
    }

    public class RefEntity
    {
        public Core.Data.Entity Entity { get; set; }
        public Guid Entityid { get; set; }
        public string Name { get; set; }
        public string Relationshipname { get; set; }
        public OperationTypeEnum? Entitystatus { get; set; }
    }
}
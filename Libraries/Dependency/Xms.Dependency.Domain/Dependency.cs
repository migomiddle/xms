using PetaPoco;
using System;

namespace Xms.Dependency.Domain
{
    [TableName("Dependency")]
    [PrimaryKey("DependencyId", AutoIncrement = false)]
    public class Dependency
    {
        public Guid DependencyId { get; set; } = Guid.NewGuid();
        public int DependentComponentType { get; set; }
        public Guid DependentObjectId { get; set; }
        public Guid RequiredObjectId { get; set; }
        public int RequiredComponentType { get; set; }
    }
}
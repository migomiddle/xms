using PetaPoco;
using System;
using Xms.Core;

namespace Xms.Security.Domain
{
    [TableName("ResourceOwner")]
    [PrimaryKey("ResourceOwnerId", AutoIncrement = false)]
    public class ResourceOwner
    {
        public Guid ResourceOwnerId { get; set; } = Guid.NewGuid();

        public string ResourceEndpoint { get; set; }

        public string PrivilegeStateEndpoint { get; set; }

        public string UIEndpoint { get; set; }

        public string ModuleName { get; set; }

        public RecordState StateCode { get; set; }

        [Ignore]
        public string ModuleLocalizedName { get; set; }
    }
}
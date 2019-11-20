using System;

namespace Xms.Sdk.Abstractions
{
    public sealed class OwnerObject
    {
        public OwnerObject(OwnerTypes ownerType, Guid ownerId)
        {
            this.OwnerType = ownerType;
            this.OwnerId = ownerId;
        }

        public OwnerTypes OwnerType { get; set; }

        public Guid OwnerId { get; set; }
    }
}
using System;
using System.Collections.Generic;

namespace Xms.Sdk.Client
{
    public interface IEntityValidator
    {
        void VerifyValues(Core.Data.Entity entity, Schema.Domain.Entity entityMetadata, List<Schema.Domain.Attribute> attributeMetadatas, Action<string> onError);
    }
}
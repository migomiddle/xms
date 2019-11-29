using System;
using Xms.Core.Context;
using Xms.DataMapping.Domain;

namespace Xms.DataMapping
{
    public interface IAttributeMapUpdater
    {
        bool Update(Func<UpdateContext<AttributeMap>, UpdateContext<AttributeMap>> context);

        bool Update(AttributeMap entity);
    }
}
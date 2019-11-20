using System;
using Xms.Core.Context;

namespace Xms.Schema.StringMap
{
    public interface IStringMapUpdater
    {
        bool Update(Func<UpdateContext<Domain.StringMap>, UpdateContext<Domain.StringMap>> context);

        bool Update(Domain.StringMap entity);
    }
}
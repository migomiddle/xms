using System;
using Xms.Core.Context;

namespace Xms.WebResource
{
    public interface IWebResourceUpdater
    {
        bool Update(Func<UpdateContext<Domain.WebResource>, UpdateContext<Domain.WebResource>> context);

        bool Update(Domain.WebResource entity);
    }
}
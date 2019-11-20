using System;
using Xms.Core.Context;

namespace Xms.Schema.OptionSet
{
    public interface IOptionSetUpdater
    {
        bool Update(Func<UpdateContext<Domain.OptionSet>, UpdateContext<Domain.OptionSet>> context);

        bool Update(Domain.OptionSet entity);
    }
}
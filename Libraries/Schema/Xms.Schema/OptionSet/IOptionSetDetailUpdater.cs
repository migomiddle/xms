using System;
using Xms.Core.Context;

namespace Xms.Schema.OptionSet
{
    public interface IOptionSetDetailUpdater
    {
        bool Update(Func<UpdateContext<Domain.OptionSetDetail>, UpdateContext<Domain.OptionSetDetail>> context);

        bool Update(Domain.OptionSetDetail entity);
    }
}
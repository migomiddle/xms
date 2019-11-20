using System.Collections.Generic;

namespace Xms.Schema.OptionSet
{
    public interface IOptionSetDetailCreater
    {
        bool Create(Domain.OptionSetDetail entity);

        bool CreateMany(List<Domain.OptionSetDetail> entities);
    }
}
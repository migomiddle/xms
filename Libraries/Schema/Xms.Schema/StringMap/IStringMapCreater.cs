using System.Collections.Generic;

namespace Xms.Schema.StringMap
{
    public interface IStringMapCreater
    {
        bool Create(Domain.StringMap entity);

        bool CreateMany(IEnumerable<Domain.StringMap> entities);
    }
}
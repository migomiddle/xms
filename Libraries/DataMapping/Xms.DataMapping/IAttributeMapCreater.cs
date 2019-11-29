using System.Collections.Generic;
using Xms.DataMapping.Domain;

namespace Xms.DataMapping
{
    public interface IAttributeMapCreater
    {
        bool Create(AttributeMap entity);

        bool CreateMany(List<AttributeMap> entities);
    }
}
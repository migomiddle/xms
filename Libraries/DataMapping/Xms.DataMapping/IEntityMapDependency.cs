using System;
using Xms.DataMapping.Domain;

namespace Xms.DataMapping
{
    public interface IEntityMapDependency
    {
        bool Create(EntityMap entity);

        bool Delete(params Guid[] id);

        bool Update(EntityMap entity);
    }
}
using System;

namespace Xms.Schema.Attribute
{
    public interface IAttributeDependency
    {
        bool Create(Domain.Attribute entity);

        bool Delete(params Guid[] id);

        bool Update(Domain.Attribute entity);
    }
}
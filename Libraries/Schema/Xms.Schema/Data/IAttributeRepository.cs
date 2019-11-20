using Xms.Core.Data;

namespace Xms.Schema.Data
{
    public interface IAttributeRepository : IRepository<Domain.Attribute>
    {
        bool Delete(Domain.Attribute entity);
    }
}
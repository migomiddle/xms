using System.Collections.Generic;

namespace Xms.Schema.Attribute
{
    public interface IDefaultAttributeProvider
    {
        List<Domain.RelationShip> GetSysAttributeRelationShips(Domain.Entity entity, List<Domain.Attribute> sysAttributes);

        List<Domain.Attribute> GetSysAttributes(Domain.Entity entity);
    }
}
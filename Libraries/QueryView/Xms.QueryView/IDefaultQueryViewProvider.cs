using System.Collections.Generic;

namespace Xms.QueryView
{
    public interface IDefaultQueryViewProvider
    {
        (Domain.QueryView DefaultView, List<Dependency.Domain.Dependency> Dependents) Get(Schema.Domain.Entity entity, List<Schema.Domain.Attribute> attributes);
    }
}
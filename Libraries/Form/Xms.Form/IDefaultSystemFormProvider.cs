using System.Collections.Generic;

namespace Xms.Form
{
    public interface IDefaultSystemFormProvider
    {
        (Domain.SystemForm DefaultForm, List<Dependency.Domain.Dependency> Dependents) Get(Schema.Domain.Entity entity);
    }
}
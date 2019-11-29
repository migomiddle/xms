using System;
using Xms.Form.Domain;

namespace Xms.Form
{
    public interface ISystemFormDependency
    {
        bool Create(SystemForm entity);

        bool Delete(params Guid[] id);

        bool Update(SystemForm entity);
    }
}
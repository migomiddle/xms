using System;
using Xms.Business.SerialNumber.Domain;

namespace Xms.Business.SerialNumber
{
    public interface ISerialNumberDependency
    {
        bool Create(SerialNumberRule entity);

        bool Delete(params Guid[] id);

        bool Update(SerialNumberRule entity);
    }
}
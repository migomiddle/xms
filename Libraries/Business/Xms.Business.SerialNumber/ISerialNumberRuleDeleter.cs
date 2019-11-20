using System;

namespace Xms.Business.SerialNumber
{
    public interface ISerialNumberRuleDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}
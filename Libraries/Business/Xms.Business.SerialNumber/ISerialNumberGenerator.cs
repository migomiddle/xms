using System;

namespace Xms.Business.SerialNumber
{
    public interface ISerialNumberGenerator
    {
        string Generate(Guid ruleid);
    }
}
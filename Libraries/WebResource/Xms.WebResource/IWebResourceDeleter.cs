using System;

namespace Xms.WebResource
{
    public interface IWebResourceDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}
using System;

namespace Xms.Schema.OptionSet
{
    public interface IOptionSetDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}
using System;

namespace Xms.Schema.OptionSet
{
    public interface IOptionSetDetailDeleter
    {
        bool DeleteById(params Guid[] id);
    }
}
using System;

namespace Xms.Data.Import
{
    public interface IFileTemplateProvider
    {
        string Get(Guid entityId);
    }
}
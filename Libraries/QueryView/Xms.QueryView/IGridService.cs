using System.Collections.Generic;
using Xms.QueryView.Abstractions.Component;

namespace Xms.QueryView
{
    public interface IGridService
    {
        GridDescriptor Build(Domain.QueryView view, List<Schema.Domain.Entity> entities, List<Schema.Domain.Attribute> attributes);
    }
}
using System;
using System.Collections.Generic;

namespace Xms.Schema.Attribute
{
    public interface IAttributeImporter
    {
        bool Import(Guid solutionId, Domain.Entity entity, List<Domain.Attribute> attributes);
    }
}
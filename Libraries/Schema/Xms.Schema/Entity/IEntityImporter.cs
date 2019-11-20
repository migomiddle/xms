using System;
using System.Collections.Generic;

namespace Xms.Schema.Entity
{
    public interface IEntityImporter
    {
        bool Import(Guid solutionId, IEnumerable<Domain.Entity> entities);
    }
}
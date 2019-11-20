using System;
using System.Collections.Generic;

namespace Xms.WebResource
{
    public interface IWebResourceImporter
    {
        bool Import(Guid solutionId, List<Domain.WebResource> webResources);
    }
}
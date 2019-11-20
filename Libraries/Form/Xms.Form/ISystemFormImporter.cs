using System;
using System.Collections.Generic;

namespace Xms.Form
{
    public interface ISystemFormImporter
    {
        bool Import(Guid solutionId, List<Domain.SystemForm> systemForms);
    }
}
using System;
using System.Collections.Generic;

namespace Xms.Schema.OptionSet
{
    public interface IOptionSetImporter
    {
        bool Import(Guid solutionId, List<Domain.OptionSet> optionSets);
    }
}
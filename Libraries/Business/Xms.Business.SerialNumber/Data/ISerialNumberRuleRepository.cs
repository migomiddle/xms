using System;
using Xms.Business.SerialNumber.Domain;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.Business.SerialNumber.Data
{
    public interface ISerialNumberRuleRepository : IRepository<SerialNumberRule>
    {
        string GetSerialNumber(Guid ruleid);

        PagedList<SerialNumberRule> QueryPaged(QueryDescriptor<SerialNumberRule> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}
using System;
using Xms.Business.FormStateRule.Domain;
using Xms.Core.Context;
using Xms.Core.Data;

namespace Xms.Business.FormStateRule.Data
{
    public interface ISystemFormStateRuleRepository : IRepository<SystemFormStateRule>
    {
        PagedList<SystemFormStateRule> QueryPaged(QueryDescriptor<SystemFormStateRule> q, int solutionComponentType, Guid solutionId, bool existInSolution);
    }
}
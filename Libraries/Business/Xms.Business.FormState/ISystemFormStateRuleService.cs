using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Business.FormStateRule.Domain;
using Xms.Core.Context;

namespace Xms.Business.FormStateRule
{
    public interface ISystemFormStateRuleService
    {
        bool Create(SystemFormStateRule entity);

        bool CreateMany(List<SystemFormStateRule> entities);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        SystemFormStateRule Find(Expression<Func<SystemFormStateRule, bool>> predicate);

        SystemFormStateRule FindById(Guid id);

        List<SystemFormStateRule> Query(Func<QueryDescriptor<SystemFormStateRule>, QueryDescriptor<SystemFormStateRule>> container);

        PagedList<SystemFormStateRule> QueryPaged(Func<QueryDescriptor<SystemFormStateRule>, QueryDescriptor<SystemFormStateRule>> container);

        PagedList<SystemFormStateRule> QueryPaged(Func<QueryDescriptor<SystemFormStateRule>, QueryDescriptor<SystemFormStateRule>> container, Guid solutionId, bool existInSolution);

        bool Update(Func<UpdateContext<SystemFormStateRule>, UpdateContext<SystemFormStateRule>> context);

        bool Update(SystemFormStateRule entity);
    }
}
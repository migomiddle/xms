using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;

namespace Xms.Dependency
{
    public interface IDependencyService
    {
        bool Create(int dependentComponentType, Guid dependentObjectId, int requiredComponentType, params Guid[] requiredObjectId);

        bool Create(string dependentComponentName, Guid dependentObjectId, string requiredComponentName, params Guid[] requiredObjectId);

        bool Create(Domain.Dependency entity);

        bool CreateMany(List<Domain.Dependency> entities);

        bool DeleteById(Guid id);

        bool DeleteByDependentId(int dependentComponentType, params Guid[] dependentId);

        bool DeleteByDependentId(string dependentComponentName, params Guid[] dependentId);

        bool DeleteByRequiredId(int requiredComponentType, params Guid[] requiredId);

        bool DeleteByRequiredId(string requiredComponentName, params Guid[] requiredId);

        bool DeleteById(IEnumerable<Guid> ids);

        Domain.Dependency Find(Expression<Func<Domain.Dependency, bool>> predicate);

        Domain.Dependency FindById(Guid id);

        List<Domain.Dependency> Query(Func<QueryDescriptor<Domain.Dependency>, QueryDescriptor<Domain.Dependency>> container);

        PagedList<Domain.Dependency> QueryPaged(Func<QueryDescriptor<Domain.Dependency>, QueryDescriptor<Domain.Dependency>> container);

        bool Update(Domain.Dependency entity);

        bool Update(int dependentComponentType, Guid dependentObjectId, int requiredComponentType, params Guid[] requiredObjectId);

        bool Update(string dependentComponentName, Guid dependentObjectId, string requiredComponentName, params Guid[] requiredObjectId);
    }
}
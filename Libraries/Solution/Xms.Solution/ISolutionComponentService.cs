using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Solution.Domain;

namespace Xms.Solution
{
    public interface ISolutionComponentService
    {
        bool Create(SolutionComponent entity);

        bool Create(Guid solutionId, Guid objectId, int componentType);

        bool Create(Guid solutionId, Guid objectId, string componentName);

        bool CreateMany(List<SolutionComponent> entities);

        bool DeleteById(Guid id);

        bool DeleteById(IEnumerable<Guid> ids);

        bool DeleteObject(Guid solutionid, Guid objectid, int componentType);

        bool DeleteObject(Guid solutionid, Guid objectid, string componentName);

        bool DeleteObject(Guid solutionid, int componentType, params Guid[] objectid);

        bool DeleteObject(Guid solutionid, string componentName, params Guid[] objectid);

        SolutionComponent Find(Expression<Func<SolutionComponent, bool>> predicate);

        SolutionComponent FindById(Guid id);

        List<SolutionComponent> Query(Func<QueryDescriptor<SolutionComponent>, QueryDescriptor<SolutionComponent>> container);

        List<SolutionComponent> Query(Guid solutionId, string componentTypeName);

        PagedList<SolutionComponent> QueryPaged(int page, int pageSize, Guid solutionId, string componentTypeName);

        PagedList<SolutionComponent> QueryPaged(Func<QueryDescriptor<SolutionComponent>, QueryDescriptor<SolutionComponent>> container);

        bool Update(Func<UpdateContext<SolutionComponent>, UpdateContext<SolutionComponent>> context);

        bool Update(SolutionComponent entity);
    }
}
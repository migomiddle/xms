using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IBusinessProcessFlowInstanceService
    {
        bool Create(BusinessProcessFlowInstance entity);

        bool CreateMany(List<BusinessProcessFlowInstance> entities);

        bool DeleteByEntityId(Guid entityId);

        bool DeleteById(Guid id);

        bool DeleteById(List<Guid> ids);

        bool DeleteByParentId(Guid parentid);

        BusinessProcessFlowInstance Find(Expression<Func<BusinessProcessFlowInstance, bool>> predicate);

        BusinessProcessFlowInstance FindById(Guid id);

        List<BusinessProcessFlowInstance> Query(Func<QueryDescriptor<BusinessProcessFlowInstance>, QueryDescriptor<BusinessProcessFlowInstance>> container);

        PagedList<BusinessProcessFlowInstance> QueryPaged(Func<QueryDescriptor<BusinessProcessFlowInstance>, QueryDescriptor<BusinessProcessFlowInstance>> container);
    }
}
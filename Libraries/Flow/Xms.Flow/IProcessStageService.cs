using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Xms.Core.Context;
using Xms.Flow.Domain;

namespace Xms.Flow
{
    public interface IProcessStageService
    {
        bool Create(ProcessStage entity);

        bool CreateMany(IList<ProcessStage> entities);

        bool DeleteById(params Guid[] id);

        bool DeleteByParentId(Guid parentid);

        ProcessStage Find(Expression<Func<ProcessStage, bool>> predicate);

        ProcessStage FindById(Guid id);

        List<ProcessStage> Query(Func<QueryDescriptor<ProcessStage>, QueryDescriptor<ProcessStage>> container);

        PagedList<ProcessStage> QueryPaged(Func<QueryDescriptor<ProcessStage>, QueryDescriptor<ProcessStage>> container);

        bool Update(Func<UpdateContext<ProcessStage>, UpdateContext<ProcessStage>> context);

        bool Update(ProcessStage entity);
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Solution.Abstractions;
using Xms.Solution.Data;

namespace Xms.Solution
{
    /// <summary>
    /// 解决方案服务
    /// </summary>
    public class SolutionService : ISolutionService
    {
        private readonly ISolutionRepository _solutionRepository;
        private readonly IAppContext _appContext;

        public SolutionService(
            IAppContext appContext,
            ISolutionRepository solutionRepository
            )
        {
            _appContext = appContext;
            _solutionRepository = solutionRepository;
        }

        public bool Create(Domain.Solution entity)
        {
            entity.OrganizationId = this._appContext.OrganizationId;
            return _solutionRepository.Create(entity);
        }

        public bool Update(Domain.Solution entity)
        {
            return _solutionRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<Domain.Solution>, UpdateContext<Domain.Solution>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Domain.Solution>());
            return _solutionRepository.Update(ctx);
        }

        public Domain.Solution FindById(Guid id)
        {
            return _solutionRepository.FindById(id);
        }

        public Domain.Solution Find(Expression<Func<Domain.Solution, bool>> predicate)
        {
            return _solutionRepository.Find(predicate);
        }

        public bool DeleteById(Guid id)
        {
            return _solutionRepository.DeleteMany(x => x.SolutionId == id && x.SolutionId != SolutionDefaults.DefaultSolutionId);
        }

        public bool DeleteById(IEnumerable<Guid> ids)
        {
            return _solutionRepository.DeleteMany(ids);
        }

        public PagedList<Domain.Solution> QueryPaged(Func<QueryDescriptor<Domain.Solution>, QueryDescriptor<Domain.Solution>> container)
        {
            QueryDescriptor<Domain.Solution> q = container(QueryDescriptorBuilder.Build<Domain.Solution>());

            return _solutionRepository.QueryPaged(q);
        }

        public List<Domain.Solution> Query(Func<QueryDescriptor<Domain.Solution>, QueryDescriptor<Domain.Solution>> container)
        {
            QueryDescriptor<Domain.Solution> q = container(QueryDescriptorBuilder.Build<Domain.Solution>());

            return _solutionRepository.Query(q)?.ToList();
        }
    }
}
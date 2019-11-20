using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Organization.Data;
using Xms.Organization.Domain;

namespace Xms.Organization
{
    /// <summary>
    /// 团队服务
    /// </summary>
    public class TeamService
    {
        public readonly ITeamRepository _teamRepository;

        public TeamService(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public bool Create(Team entity)
        {
            return _teamRepository.Create(entity);
        }

        public bool Update(Team entity)
        {
            return _teamRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<Team>, UpdateContext<Team>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<Team>());
            return _teamRepository.Update(ctx);
        }

        public Team FindById(Guid id)
        {
            return _teamRepository.FindById(id);
        }

        public bool DeleteById(Guid id)
        {
            var deletedRecord = this.FindById(id);
            if (deletedRecord != null)
            {
                return _teamRepository.DeleteById(id);
            }
            return false;
        }

        public bool DeleteById(List<Guid> ids)
        {
            foreach (var id in ids)
            {
                this.DeleteById(id);
            }
            return true;
        }

        public PagedList<Team> QueryPaged(Func<QueryDescriptor<Team>, QueryDescriptor<Team>> container)
        {
            QueryDescriptor<Team> q = container(QueryDescriptorBuilder.Build<Team>());

            return _teamRepository.QueryPaged(q);
        }

        public List<Team> Query(Func<QueryDescriptor<Team>, QueryDescriptor<Team>> container)
        {
            QueryDescriptor<Team> q = container(QueryDescriptorBuilder.Build<Team>());
            return _teamRepository.Query(q)?.ToList();
        }
    }
}
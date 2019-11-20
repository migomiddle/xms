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
    /// 团队成员服务
    /// </summary>
    public class TeamMembershipService
    {
        public readonly ITeamMembershipRepository _teamMembershipRepository;

        public TeamMembershipService(ITeamMembershipRepository teamMembershipRepository)
        {
            _teamMembershipRepository = teamMembershipRepository;
        }

        public bool Create(TeamMembership entity)
        {
            return _teamMembershipRepository.Create(entity);
        }

        public bool Update(TeamMembership entity)
        {
            return _teamMembershipRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<TeamMembership>, UpdateContext<TeamMembership>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<TeamMembership>());
            return _teamMembershipRepository.Update(ctx);
        }

        public TeamMembership FindById(Guid id)
        {
            return _teamMembershipRepository.FindById(id);
        }

        public bool DeleteById(Guid id)
        {
            var deletedRecord = this.FindById(id);
            if (deletedRecord != null)
            {
                return _teamMembershipRepository.DeleteById(id);
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

        public PagedList<TeamMembership> QueryPaged(Func<QueryDescriptor<TeamMembership>, QueryDescriptor<TeamMembership>> container)
        {
            QueryDescriptor<TeamMembership> q = container(QueryDescriptorBuilder.Build<TeamMembership>());

            return _teamMembershipRepository.QueryPaged(q);
        }

        public List<TeamMembership> Query(Func<QueryDescriptor<TeamMembership>, QueryDescriptor<TeamMembership>> container)
        {
            QueryDescriptor<TeamMembership> q = container(QueryDescriptorBuilder.Build<TeamMembership>());
            return _teamMembershipRepository.Query(q)?.ToList();
        }
    }
}
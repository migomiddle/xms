using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Authorization.Abstractions;
using Xms.Context;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Flow.Abstractions;
using Xms.Flow.Core;
using Xms.Flow.Data;
using Xms.Flow.Domain;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;

namespace Xms.Flow
{
    /// <summary>
    /// 流程查找服务
    /// </summary>
    public class WorkFlowFinder : IWorkFlowFinder
    {
        private readonly IWorkFlowRepository _workFlowRepository;

        //private readonly ILocalizedLabelService _localizedLabelService;
        private readonly IRoleObjectAccessService _roleObjectAccessService;

        private readonly IAppContext _appContext;
        private readonly ICurrentUser _currentUser;

        public WorkFlowFinder(IAppContext appContext
            , IWorkFlowRepository workFlowRepository
            //, ILocalizedLabelService localizedLabelService
            , IRoleObjectAccessService roleObjectAccessService)
        {
            _appContext = appContext;
            _workFlowRepository = workFlowRepository;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            //_localizedLabelService = localizedLabelService;
            _roleObjectAccessService = roleObjectAccessService;
        }

        public WorkFlow FindById(Guid id)
        {
            var data = _workFlowRepository.FindById(id);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public WorkFlow Find(Expression<Func<WorkFlow, bool>> predicate)
        {
            var data = _workFlowRepository.Find(predicate);
            if (data != null)
            {
                WrapLocalizedLabel(data);
            }
            return data;
        }

        public PagedList<WorkFlow> QueryPaged(Func<QueryDescriptor<WorkFlow>, QueryDescriptor<WorkFlow>> container)
        {
            QueryDescriptor<WorkFlow> q = container(QueryDescriptorBuilder.Build<WorkFlow>());
            var datas = _workFlowRepository.QueryPaged(q);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public PagedList<WorkFlow> QueryPaged(Func<QueryDescriptor<WorkFlow>, QueryDescriptor<WorkFlow>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<WorkFlow> q = container(QueryDescriptorBuilder.Build<WorkFlow>());
            var datas = _workFlowRepository.QueryPaged(q, ModuleCollection.GetIdentity(WorkFlowDefaults.ModuleName), solutionId, existInSolution);

            WrapLocalizedLabel(datas.Items);
            return datas;
        }

        public List<WorkFlow> Query(Func<QueryDescriptor<WorkFlow>, QueryDescriptor<WorkFlow>> container)
        {
            QueryDescriptor<WorkFlow> q = container(QueryDescriptorBuilder.Build<WorkFlow>());
            var datas = _workFlowRepository.Query(q)?.ToList();

            WrapLocalizedLabel(datas);
            return datas;
        }

        public List<WorkFlow> QueryAuthorized(Guid entityid, FlowType category)
        {
            var q = QueryDescriptorBuilder.Build<WorkFlow>();
            q.Where(f => f.EntityId == entityid);
            var datas = _workFlowRepository.Query(x => x.EntityId == entityid && x.StateCode == Xms.Core.RecordState.Enabled && x.Category == (int)category)?.ToList();
            if (!_currentUser.IsSuperAdmin && datas.NotEmpty())
            {
                var authIds = datas.Where(x => x.AuthorizationEnabled).Select(x => x.WorkFlowId).ToArray();
                if (authIds.NotEmpty())
                {
                    var authorizedItems = _roleObjectAccessService.Authorized(WorkFlowDefaults.ModuleName, authIds);
                    datas.RemoveAll(x => x.AuthorizationEnabled && !authorizedItems.Contains(x.WorkFlowId));
                }
            }
            WrapLocalizedLabel(datas);
            return datas;
        }

        private void WrapLocalizedLabel(IEnumerable<WorkFlow> datas)
        {
            //if (datas.NotEmpty())
            //{
            //    var ids = datas.Select(f => f.WorkFlowId);
            //    var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == _currentUser.UserSettings.LanguageId && f.ObjectId.In(ids)));
            //    foreach (var d in datas)
            //    {
            //        d.Name = _localizedLabelService.GetLabelText(labels, d.WorkFlowId, "LocalizedName", d.Name);
            //        d.Description = _localizedLabelService.GetLabelText(labels, d.WorkFlowId, "Description", d.Description);
            //    }
            //}
        }

        private void WrapLocalizedLabel(WorkFlow entity)
        {
            //var labels = _localizedLabelService.Query(n => n.Where(f => f.LanguageId == _currentUser.UserSettings.LanguageId && f.ObjectId == entity.WorkFlowId));
            //entity.Name = _localizedLabelService.GetLabelText(labels, entity.WorkFlowId, "LocalizedName", entity.Name);
            //entity.Description = _localizedLabelService.GetLabelText(labels, entity.WorkFlowId, "Description", entity.Description);
        }
    }
}
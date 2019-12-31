using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Core;
using Xms.Core.Context;
using Xms.Data.Provider;
using Xms.Flow;
using Xms.Flow.Abstractions;
using Xms.Flow.Core;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Security.Abstractions;
using Xms.Solution.Abstractions;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Infrastructure;

namespace Xms.Web.Api
{
    /// <summary>
    /// 工作流接口
    /// </summary>
    [Route("{org}/api/[controller]")]
    public class WorkFlowController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IWorkFlowFinder _workFlowFinder;
        private readonly IWorkFlowUpdater _workFlowUpdater;
        private readonly IWorkFlowInstanceService _workFlowInstanceService;
        private readonly IWorkFlowProcessFinder _workFlowProcessFinder;

        public WorkFlowController(IWebAppContext appContext
            , IWorkFlowFinder workFlowFinder
            , IWorkFlowUpdater workFlowUpdater
            , IEntityFinder entityService
            , IWorkFlowInstanceService workFlowInstanceService
            , IWorkFlowProcessFinder workFlowProcessFinder)
            : base(appContext)
        {
            _workFlowFinder = workFlowFinder;
            _workFlowUpdater = workFlowUpdater;
            _entityFinder = entityService;
            _workFlowInstanceService = workFlowInstanceService;
            _workFlowProcessFinder = workFlowProcessFinder;
        }

        [Description("获取流程信息")]
        [HttpGet("{id}")]
        public IActionResult Get(Guid id)
        {
            return JOk(_workFlowFinder.FindById(id));
        }

        [Description("流程执行详情")]
        [HttpGet("Instances")]
        public IActionResult Instances(Guid entityid, Guid recordid)
        {
            var instances = _workFlowInstanceService.Query(n => n
                .Where(f => f.EntityId == entityid && f.ObjectId == recordid)
                .Sort(s => s.SortDescending(f => f.CreatedOn))
            );
            if (instances.IsEmpty())
            {
                return NotFound();
            }
            var entityMeta = _entityFinder.FindById(entityid);
            if (entityMeta == null)
            {
                return NotFound();
            }
            foreach (var instance in instances)
            {
                var steps = _workFlowProcessFinder.Query(n => n
                .Where(f => f.WorkFlowInstanceId == instance.WorkFlowInstanceId && f.StateCode != WorkFlowProcessState.Disabled)
                .Sort(s => s.SortAscending(f => f.StepOrder)).Sort(s => s.SortAscending(f => f.StateCode)));
                instance.Steps = steps;
            }
            return JOk(instances);
        }

        [Description("流程执行详情")]
        [HttpGet("Current")]
        public IActionResult Current(Guid entityid, Guid recordid)
        {
            var instances = _workFlowInstanceService.Top(n => n.Take(1)
                .Where(f => f.EntityId == entityid && f.ObjectId == recordid)
                .Sort(s => s.SortDescending(f => f.CreatedOn))
            );
            if (instances.IsEmpty())
            {
                return NotFound();
            }
            foreach (var instance in instances)
            {
                var steps = _workFlowProcessFinder.Query(n => n
                .Where(f => f.WorkFlowInstanceId == instance.WorkFlowInstanceId && f.StateCode != WorkFlowProcessState.Disabled)
                .Sort(s => s.SortAscending(f => f.StepOrder)).Sort(s => s.SortAscending(f => f.StateCode)));
                instance.Steps = steps;
            }
            return JOk(instances);
        }

        [Description("解决方案组件")]
        [HttpGet("SolutionComponents")]
        public IActionResult SolutionComponents([FromQuery]GetSolutionComponentsModel model)
        {
            var data = _workFlowFinder.QueryPaged(x => x.Page(model.Page, model.PageSize), model.SolutionId, model.InSolution);
            if (data.Items.NotEmpty())
            {
                var result = data.Items.Select(x => (new SolutionComponentItem { ObjectId = x.WorkFlowId, Name = x.Name, LocalizedName = x.Name, ComponentTypeName = WorkFlowDefaults.ModuleName, CreatedOn = x.CreatedOn })).ToList();
                return JOk(new PagedList<SolutionComponentItem>()
                {
                    CurrentPage = model.Page
                    ,
                    ItemsPerPage = model.PageSize
                    ,
                    Items = result
                    ,
                    TotalItems = data.TotalItems
                    ,
                    TotalPages = data.TotalPages
                });
            }
            return JOk(data);
        }

        [Description("查询流程权限资源")]
        [HttpGet("PrivilegeResource")]
        public IActionResult PrivilegeResource(bool? authorizationEnabled)
        {
            FilterContainer<Flow.Domain.WorkFlow> filter = FilterContainerBuilder.Build<Flow.Domain.WorkFlow>();
            filter.And(x => x.StateCode == RecordState.Enabled);
            if (authorizationEnabled.HasValue)
            {
                filter.And(x => x.AuthorizationEnabled == authorizationEnabled.Value);
            }
            var data = _workFlowFinder.Query(x => x.Select(s => new { s.WorkFlowId, s.Name, s.EntityId, s.AuthorizationEnabled }).Where(f => f.StateCode == Core.RecordState.Enabled));
            if (data.NotEmpty())
            {
                var result = new List<PrivilegeResourceItem>();
                var entities = _entityFinder.FindAll();
                foreach (var item in entities)
                {
                    var attributes = data.Where(x => x.EntityId == item.EntityId);
                    if (!attributes.Any())
                    {
                        continue;
                    }
                    var group1 = new PrivilegeResourceItem();
                    group1.Label = item.LocalizedName;
                    group1.Children = attributes.Select(x => (new PrivilegeResourceItem { Id = x.WorkFlowId, Label = x.Name, AuthorizationEnabled = x.AuthorizationEnabled })).ToList();
                    result.Add(group1);
                }
                return JOk(result);
            }
            return JOk();
        }

        [Description("启用流程权限")]
        [HttpPost("AuthorizationEnabled")]
        public IActionResult AuthorizationEnabled(UpdateAuthorizationStateModel model)
        {
            var authorizations = _workFlowFinder.Query(x => x.Where(w => w.AuthorizationEnabled == true));
            if (authorizations.NotEmpty())
            {
                _workFlowUpdater.UpdateAuthorization(false, authorizations.Select(x => x.WorkFlowId).ToArray());
            }
            if (Arguments.HasValue(model.ObjectId))
            {
                _workFlowUpdater.UpdateAuthorization(true, model.ObjectId);
            }
            return SaveSuccess();
        }
    }
}
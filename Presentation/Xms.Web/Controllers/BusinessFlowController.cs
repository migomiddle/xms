using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Xms.Core;
using Xms.Core.Data;
using Xms.Flow;
using Xms.Flow.Abstractions;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Attribute;
using Xms.Schema.Entity;
using Xms.Schema.RelationShip;
using Xms.Sdk.Client;
using Xms.Sdk.Extensions;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 业务流程控制器
    /// </summary>
    [Route("{org}/flow/[action]")]
    public class BusinessProcessController : AuthenticatedControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IAttributeFinder _attributeFinder;
        private readonly IRelationShipFinder _relationShipFinder;
        private readonly IWorkFlowFinder _workFlowFinder;
        private readonly IBusinessProcessFlowInstanceService _businessProcessFlowInstanceService;
        private readonly IProcessStageService _processStageService;
        private readonly IDataFinder _dataFinder;

        public BusinessProcessController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IAttributeFinder attributeFinder
            , IRelationShipFinder relationShipFinder
            , IWorkFlowFinder workFlowFinder
            , IBusinessProcessFlowInstanceService businessProcessFlowInstanceService
            , IProcessStageService processStageService
            , IDataFinder dataFinder)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _attributeFinder = attributeFinder;
            _relationShipFinder = relationShipFinder;
            _workFlowFinder = workFlowFinder;
            _businessProcessFlowInstanceService = businessProcessFlowInstanceService;
            _processStageService = processStageService;
            _dataFinder = dataFinder;
        }

        [Description("业务流程")]
        public IActionResult BusinessProcess([FromBody]BusinessProcessArgsModel args)
        {
            if (args.EntityId.Equals(Guid.Empty) || args.RecordId.Equals(Guid.Empty))
            {
                return JError(T["parameter_error"]);
            }
            var entityMeta = _entityFinder.FindById(args.EntityId);
            if (entityMeta == null)
            {
                return NotFound();
            }
            if (!entityMeta.BusinessFlowEnabled)
            {
                return JError(T["businessflow_disabled"]);
            }
            var data = this._dataFinder.RetrieveById(entityMeta.Name, args.RecordId);
            if (data.IsEmpty())
            {
                return NotFound();
            }
            WorkFlow flowInfo = null;
            BusinessProcessFlowInstance flowInstance = null;
            List<ProcessStage> stages = null;
            Guid entityStageId = data.GetGuidValue("stageid");
            int entityIndex = 0;
            if (args.BusinessflowId.HasValue && !args.BusinessflowId.Equals(Guid.Empty))
            {
                flowInfo = _workFlowFinder.Find(n => n.WorkFlowId == args.BusinessflowId.Value);
            }
            else if (args.BusinessflowInstanceId.HasValue && !args.BusinessflowInstanceId.Value.Equals(Guid.Empty))
            {
                flowInstance = _businessProcessFlowInstanceService.FindById(args.BusinessflowInstanceId.Value);
                if (flowInstance != null)
                {
                    flowInfo = _workFlowFinder.Find(n => n.WorkFlowId == flowInstance.WorkFlowId);
                }
            }
            if (flowInfo == null)
            {
                var flowList = _workFlowFinder.QueryAuthorized(args.EntityId, FlowType.Business);
                flowInfo = flowList.NotEmpty() ? flowList.First() : null;
                if (flowInfo == null && !entityStageId.Equals(Guid.Empty))
                {
                    //查找当前实体所在阶段
                    var processstage = _processStageService.Find(n => n.ProcessStageId == entityStageId);
                    if (processstage == null)
                    {
                        return NotFound();
                    }
                    flowInfo = _workFlowFinder.Find(n => n.WorkFlowId == processstage.WorkFlowId && n.StateCode == RecordState.Enabled);
                }
            }
            if (flowInfo == null)
            {
                return Content("");
            }
            stages = _processStageService.Query(n => n
                    .Where(f => f.WorkFlowId == flowInfo.WorkFlowId)
                    .Sort(s => s.SortAscending(f => f.StageOrder))
                    );
            var entityIds = stages.Select(n => n.EntityId).Distinct().ToList();
            entityIndex = entityIds.FindIndex(n => n.Equals(args.EntityId)) + 1;
            //查询业务流程实例
            if (flowInstance == null)
            {
                if (entityIndex == 1)
                {
                    flowInstance = _businessProcessFlowInstanceService.Find(n => n.WorkFlowId == flowInfo.WorkFlowId && n.Entity1Id == args.RecordId);
                }

                if (entityIndex == 2)
                {
                    flowInstance = _businessProcessFlowInstanceService.Find(n => n.WorkFlowId == flowInfo.WorkFlowId && n.Entity2Id == args.RecordId);
                }

                if (entityIndex == 3)
                {
                    flowInstance = _businessProcessFlowInstanceService.Find(n => n.WorkFlowId == flowInfo.WorkFlowId && n.Entity3Id == args.RecordId);
                }

                if (entityIndex == 4)
                {
                    flowInstance = _businessProcessFlowInstanceService.Find(n => n.WorkFlowId == flowInfo.WorkFlowId && n.Entity4Id == args.RecordId);
                }

                if (entityIndex == 5)
                {
                    flowInstance = _businessProcessFlowInstanceService.Find(n => n.WorkFlowId == flowInfo.WorkFlowId && n.Entity5Id == args.RecordId);
                }
            }

            if (flowInstance != null)
            {
                entityStageId = flowInstance.ProcessStageId.Value;
            }
            BusinessProcessModel model = new BusinessProcessModel
            {
                EntityId = args.EntityId,
                RecordId = args.RecordId,
                Data = data,
                BusinessFlow = flowInfo,
                BusinessFlowInstance = flowInstance,
                Stages = stages
            };
            model.CurrentStageId = entityStageId.Equals(Guid.Empty) ? model.Stages.First().ProcessStageId : entityStageId;
            Dictionary<string, object> steps = new Dictionary<string, object>();
            List<Schema.Domain.Attribute> attributes = new List<Schema.Domain.Attribute>();
            foreach (var stage in model.Stages)
            {
                var st = new List<ProcessStep>();
                st = st.DeserializeFromJson(stage.Steps);
                steps.Add(stage.ProcessStageId.ToString(), st);
                var attrs = st.Select(f => f.AttributeName).ToList();
                attributes.AddRange(_attributeFinder.Query(n => n.Where(f => f.Name.In(attrs) && f.EntityId == stage.EntityId)));
            }
            model.Steps = steps;
            model.Attributes = attributes;
            var related = model.Stages.Where(n => n.RelationshipName.IsNotEmpty()).ToList();
            if (related.NotEmpty())
            {
                var rnames = related.Select(f => f.RelationshipName).ToList();
                model.RelationShips = _relationShipFinder.Query(n => n.Where(f => f.Name.In(rnames)));
                _relationShipFinder.WrapLocalizedLabel(model.RelationShips);
            }
            if (model.BusinessFlowInstance != null)
            {
                var rsRecords = new Dictionary<string, object>();
                int i = 1;
                foreach (var eid in entityIds)
                {
                    var eidMeta = _entityFinder.FindById(eid);
                    var filter = new Dictionary<string, object>();
                    if (i == 1 && flowInstance.Entity1Id.HasValue && !flowInstance.Entity1Id.Value.Equals(Guid.Empty))
                    {
                        filter.Add(eidMeta.Name + "id", flowInstance.Entity1Id);
                    }

                    if (i == 2 && flowInstance.Entity2Id.HasValue && !flowInstance.Entity2Id.Value.Equals(Guid.Empty))
                    {
                        filter.Add(eidMeta.Name + "id", flowInstance.Entity2Id);
                    }

                    if (i == 3 && flowInstance.Entity3Id.HasValue && !flowInstance.Entity3Id.Value.Equals(Guid.Empty))
                    {
                        filter.Add(eidMeta.Name + "id", flowInstance.Entity3Id);
                    }

                    if (i == 4 && flowInstance.Entity4Id.HasValue && !flowInstance.Entity4Id.Value.Equals(Guid.Empty))
                    {
                        filter.Add(eidMeta.Name + "id", flowInstance.Entity4Id);
                    }

                    if (i == 5 && flowInstance.Entity5Id.HasValue && !flowInstance.Entity5Id.Value.Equals(Guid.Empty))
                    {
                        filter.Add(eidMeta.Name + "id", flowInstance.Entity5Id);
                    }

                    if (filter.Count > 0)
                    {
                        rsRecords.Add(eid.ToString(), _dataFinder.RetrieveByAttribute(eidMeta.Name, filter));
                    }
                    else
                    {
                        rsRecords.Add(eid.ToString(), null);
                    }

                    i++;
                }
                model.RelatedRecords = rsRecords;
            }
            return View($"~/Views/Flow/{WebContext.ActionName}.cshtml", model);
        }
    }
}
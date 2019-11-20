using Microsoft.AspNetCore.Mvc;
using System;
using System.ComponentModel;
using System.Linq;
using Xms.Core.Data;
using Xms.DataMapping;
using Xms.Flow;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Sdk.Client;
using Xms.Web.Api.Models;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;

namespace Xms.Web.Api
{
    /// <summary>
    /// 业务流程控制器
    /// </summary>
    [Route("{org}/api/flow/[action]")]
    public class BusinessFlowController : ApiControllerBase
    {
        private readonly IEntityFinder _entityFinder;
        private readonly IEntityMapFinder _entityMapFinder;
        private readonly IWorkFlowFinder _workFlowFinder;
        private readonly IBusinessProcessFlowInstanceService _businessProcessFlowInstanceService;
        private readonly IBusinessProcessFlowInstanceUpdater _businessProcessFlowInstanceUpdater;
        private readonly IProcessStageService _processStageService;
        private readonly IDataFinder _dataFinder;
        private readonly IDataUpdater _dataUpdater;
        private readonly IDataMapper _dataMapper;

        public BusinessFlowController(IWebAppContext appContext
            , IEntityFinder entityFinder
            , IEntityMapFinder entityMapFinder
            , IWorkFlowFinder workFlowFinder
            , IBusinessProcessFlowInstanceService businessProcessFlowInstanceService
            , IBusinessProcessFlowInstanceUpdater businessProcessFlowInstanceUpdater
            , IProcessStageService processStageService
            , IDataFinder dataFinder
            , IDataUpdater dataUpdater
            , IDataMapper dataMapper)
            : base(appContext)
        {
            _entityFinder = entityFinder;
            _entityMapFinder = entityMapFinder;
            _workFlowFinder = workFlowFinder;
            _businessProcessFlowInstanceService = businessProcessFlowInstanceService;
            _businessProcessFlowInstanceUpdater = businessProcessFlowInstanceUpdater;
            _processStageService = processStageService;
            _dataFinder = dataFinder;
            _dataUpdater = dataUpdater;
            _dataMapper = dataMapper;
        }

        [Description("业务流程推进并生成一对多单据")]
        public IActionResult ForwardAndAppend(Guid entityid, Guid referencedrecordid, Guid workflowid, Guid stageid, Guid? instanceId, string relationshipname)
        {
            if (entityid.Equals(Guid.Empty) || referencedrecordid.Equals(Guid.Empty) || workflowid.Equals(Guid.Empty) || stageid.Equals(Guid.Empty))
            {
                return JError(T["parameter_error"]);
            }
            var entityMeta = _entityFinder.FindById(entityid);
            if (entityMeta == null)
            {
                return NotFound();
            }
            var flowInfo = _workFlowFinder.Find(n => n.WorkFlowId == workflowid);
            if (flowInfo == null)
            {
                return NotFound();
            }
            var stages = _processStageService.Query(n => n
            .Where(f => f.WorkFlowId == flowInfo.WorkFlowId)
            .Sort(s => s.SortAscending(f => f.StageOrder)));

            var currentStage = stages.Find(n => n.ProcessStageId == stageid);
            var prevStage = stages.Find(n => n.StageOrder == currentStage.StageOrder - 1);
            if (currentStage.EntityId.Equals(prevStage.EntityId))
            {
                return JError(T["parameter_error"]);
            }
            BusinessProcessFlowInstance bpfInstance = null;
            if (instanceId.HasValue && !instanceId.Value.Equals(Guid.Empty))
            {
                bpfInstance = _businessProcessFlowInstanceService.Find(n => n.BusinessProcessFlowInstanceId == instanceId && n.WorkFlowId == workflowid);
                if (bpfInstance == null)
                {
                    return NotFound();
                }
            }
            if (bpfInstance == null)
            {
                instanceId = Guid.NewGuid();
                bpfInstance = new BusinessProcessFlowInstance();
                bpfInstance.BusinessProcessFlowInstanceId = instanceId.Value;
                bpfInstance.WorkFlowId = workflowid;
                bpfInstance.ProcessStageId = stages.First().ProcessStageId;
                bpfInstance.Entity1Id = referencedrecordid;
                bpfInstance.ProcessEntityId = entityid;
                _businessProcessFlowInstanceService.Create(bpfInstance);
            }
            //是否存在单据转换规则
            var entityMap = _entityMapFinder.Find(prevStage.EntityId, entityid);
            if (entityMap == null)
            {
                return JOk(new
                {
                    EntityId = entityid
                    ,
                    StageId = stageid
                    ,
                    BusinessFlowId = workflowid
                    ,
                    BusinessFlowInstanceId = instanceId.Value
                    ,
                    RelationShipName = relationshipname
                    ,
                    ReferencedRecordId = referencedrecordid
                });
            }
            var recordid = _dataMapper.Create(prevStage.EntityId, currentStage.EntityId, referencedrecordid);
            _businessProcessFlowInstanceUpdater.UpdateForward(workflowid, instanceId.Value, stageid, recordid);
            //更新当前记录的业务阶段
            var updData = new Entity(entityMeta.Name);
            updData.SetIdValue(recordid);
            updData.SetAttributeValue("stageid", stageid);
            _dataUpdater.Update(updData);

            return JOk(new
            {
                EntityId = entityid
                ,
                RecordId = recordid
                ,
                BusinessFlowId = workflowid
                ,
                BusinessFlowInstanceId = instanceId.Value
            });
        }

        [Description("更新业务流程阶段")]
        public IActionResult UpdateProcessStage(UpdateProcessStageModel model)
        {
            if (model.EntityId.Equals(Guid.Empty) || model.RecordId.Equals(Guid.Empty) || model.WorkflowId.Equals(Guid.Empty) || model.StageId.Equals(Guid.Empty))
            {
                return JError(T["parameter_error"]);
            }
            var entityMeta = _entityFinder.FindById(model.EntityId);
            if (entityMeta == null)
            {
                return NotFound();
            }
            var flowInfo = _workFlowFinder.FindById(model.WorkflowId);
            if (flowInfo == null)
            {
                return NotFound();
            }
            var data = _dataFinder.RetrieveById(entityMeta.Name, model.RecordId);
            if (data.IsEmpty())
            {
                return NotFound();
            }
            var stages = _processStageService.Query(n => n
            .Where(f => f.WorkFlowId == flowInfo.WorkFlowId)
            .Sort(s => s.SortAscending(f => f.StageOrder)));

            var currentStage = stages.Find(n => n.ProcessStageId == model.StageId);
            var entityIds = stages.Select(n => n.EntityId).Distinct().ToList();
            int entityIndex = entityIds.FindIndex(n => n.Equals(currentStage.EntityId)) + 1;
            BusinessProcessFlowInstance bpfInstance = null;
            if (model.InstanceId.HasValue && !model.InstanceId.Value.Equals(Guid.Empty))
            {
                bpfInstance = _businessProcessFlowInstanceService.Find(n => n.BusinessProcessFlowInstanceId == model.InstanceId && n.WorkFlowId == model.WorkflowId);
                if (bpfInstance == null)
                {
                    return NotFound();
                }
            }
            if (bpfInstance == null)
            {
                bpfInstance = new BusinessProcessFlowInstance();
                bpfInstance.BusinessProcessFlowInstanceId = Guid.NewGuid();
                bpfInstance.WorkFlowId = model.WorkflowId;
                bpfInstance.ProcessStageId = currentStage.ProcessStageId;
                bpfInstance.Entity1Id = model.FromRecordId;
                bpfInstance.Entity2Id = model.RecordId;
                bpfInstance.ProcessEntityId = model.EntityId;
                _businessProcessFlowInstanceService.Create(bpfInstance);
            }
            else
            {
                var originalStage = stages.Find(n => n.ProcessStageId == bpfInstance.ProcessStageId);
                var isForward = currentStage.StageOrder > originalStage.StageOrder;
                if (isForward)
                {
                    _businessProcessFlowInstanceUpdater.UpdateForward(model.WorkflowId, bpfInstance.BusinessProcessFlowInstanceId, model.StageId, model.RecordId);
                }
                //如果后退并且阶段不在同一实体，更新实例对应的记录id
                else if (!isForward)
                {
                    if (!model.InstanceId.HasValue || model.InstanceId.Equals(Guid.Empty))
                    {
                        return JError(T["parameter_error"]);
                    }
                    _businessProcessFlowInstanceUpdater.UpdateBack(model.WorkflowId, model.InstanceId.Value, model.StageId, model.RecordId);
                }
            }
            //更新当前记录的业务阶段
            var updData = new Entity(data.Name);
            updData.SetIdValue(data.GetIdValue());
            updData.SetAttributeValue("stageid", model.StageId);
            _dataUpdater.Update(updData);

            return SaveSuccess();
        }
    }
}
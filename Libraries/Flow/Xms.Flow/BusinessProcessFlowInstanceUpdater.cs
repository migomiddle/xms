using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Context;
using Xms.Core.Data;
using Xms.Data.Provider;
using Xms.Flow.Data;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;
using Xms.Schema.Entity;
using Xms.Sdk.Client;

namespace Xms.Flow
{
    /// <summary>
    /// 业务流程状态更新服务
    /// </summary>
    public class BusinessProcessFlowInstanceUpdater : IBusinessProcessFlowInstanceUpdater
    {
        private readonly IBusinessProcessFlowInstanceRepository _businessProcessFlowInstanceRepository;
        private readonly IEntityFinder _entityFinder;
        private readonly IProcessStageService _processStageService;
        private readonly IDataFinder _dataFinder;
        private readonly IDataUpdater _dataUpdater;

        public BusinessProcessFlowInstanceUpdater(IBusinessProcessFlowInstanceRepository businessProcessFlowInstanceRepository
            , IEntityFinder entityFinder
            , IProcessStageService processStageService
            , IDataFinder dataFinder
            , IDataUpdater dataUpdater)
        {
            _businessProcessFlowInstanceRepository = businessProcessFlowInstanceRepository;
            _entityFinder = entityFinder;
            _processStageService = processStageService;
            _dataFinder = dataFinder;
            _dataUpdater = dataUpdater;
        }

        public bool Update(BusinessProcessFlowInstance entity)
        {
            return _businessProcessFlowInstanceRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<BusinessProcessFlowInstance>, UpdateContext<BusinessProcessFlowInstance>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<BusinessProcessFlowInstance>());
            return _businessProcessFlowInstanceRepository.Update(ctx);
        }

        public bool UpdateBack(Guid workFlowId, Guid instanceId, Guid processStageId, Guid recordId)
        {
            var bpfInstance = _businessProcessFlowInstanceRepository.Find(n => n.BusinessProcessFlowInstanceId == instanceId && n.WorkFlowId == workFlowId);
            if (bpfInstance == null)
            {
                return false;
            }
            var stages = _processStageService.Query(n => n
            .Where(f => f.WorkFlowId == workFlowId)
            .Sort(s => s.SortAscending(f => f.StageOrder)));

            var currentStage = stages.Find(n => n.ProcessStageId == processStageId);
            var entityIds = stages.Select(n => n.EntityId).Distinct().ToList();
            int entityIndex = entityIds.FindIndex(n => n.Equals(currentStage.EntityId)) + 1;
            //var _bpfService = new BusinessProcessFlowInstanceService(User);
            var originalStage = stages.Find(n => n.ProcessStageId == bpfInstance.ProcessStageId);
            //var _organizationServiceProxy = new SDK.OrganizationServiceProxy(User);
            var eidMeta = _entityFinder.FindById(originalStage.EntityId);
            var filter = new Dictionary<string, object>();
            if (entityIndex == 1)
            {
                this.Update(n => n
                    .Set(f => f.ProcessStageId, currentStage.ProcessStageId)
                    .Set(f => f.ProcessEntityId, currentStage.EntityId)
                    .Set(f => f.Entity2Id, null)
                    .Set(f => f.Entity3Id, null)
                    .Set(f => f.Entity4Id, null)
                    .Set(f => f.Entity5Id, null)
                    .Where(w => w.BusinessProcessFlowInstanceId == bpfInstance.BusinessProcessFlowInstanceId)
                );
                filter.Add(eidMeta.Name + "id", bpfInstance.Entity2Id);
            }
            else if (entityIndex == 2)
            {
                this.Update(n => n
                    .Set(f => f.ProcessStageId, currentStage.ProcessStageId)
                    .Set(f => f.ProcessEntityId, currentStage.EntityId)
                    .Set(f => f.Entity3Id, null)
                    .Set(f => f.Entity4Id, null)
                    .Set(f => f.Entity5Id, null)
                    .Where(w => w.BusinessProcessFlowInstanceId == bpfInstance.BusinessProcessFlowInstanceId)
                );
                filter.Add(eidMeta.Name + "id", bpfInstance.Entity3Id);
            }
            else if (entityIndex == 3)
            {
                this.Update(n => n
                    .Set(f => f.ProcessStageId, currentStage.ProcessStageId)
                    .Set(f => f.ProcessEntityId, currentStage.EntityId)
                    .Set(f => f.Entity4Id, null)
                    .Set(f => f.Entity5Id, null)
                    .Where(w => w.BusinessProcessFlowInstanceId == bpfInstance.BusinessProcessFlowInstanceId)
                );
                filter.Add(eidMeta.Name + "id", bpfInstance.Entity4Id);
            }
            else if (entityIndex == 4)
            {
                this.Update(n => n
                    .Set(f => f.ProcessStageId, currentStage.ProcessStageId)
                    .Set(f => f.ProcessEntityId, currentStage.EntityId)
                    .Set(f => f.Entity5Id, null)
                    .Where(w => w.BusinessProcessFlowInstanceId == bpfInstance.BusinessProcessFlowInstanceId)
                );
                filter.Add(eidMeta.Name + "id", bpfInstance.Entity5Id);
            }
            if (!originalStage.EntityId.Equals(currentStage.EntityId))
            {
                //更新原阶段对应的实体记录的阶段
                var originalData = _dataFinder.RetrieveByAttribute(eidMeta.Name, filter);
                if (originalData.NotEmpty())
                {
                    var updOgnData = new Entity(originalData.Name);
                    updOgnData.SetIdValue(originalData.GetIdValue());
                    updOgnData.SetAttributeValue("stageid", null);
                    _dataUpdater.Update(updOgnData);
                }
            }

            return true;
        }

        public bool UpdateForward(Guid workflowId, Guid instanceId, Guid processStageId, Guid recordId)
        {
            //var _bpfService = new BusinessProcessFlowInstanceService(User);
            BusinessProcessFlowInstance bpfInstance = _businessProcessFlowInstanceRepository.Find(n => n.BusinessProcessFlowInstanceId == instanceId && n.WorkFlowId == workflowId);
            var stages = _processStageService.Query(n => n
               .Where(f => f.WorkFlowId == bpfInstance.WorkFlowId)
               .Sort(s => s.SortAscending(f => f.StageOrder)));
            var currentStage = stages.Find(n => n.ProcessStageId == processStageId);
            var prevStage = stages.Find(n => n.StageOrder == currentStage.StageOrder - 1);
            var entityId = currentStage.EntityId;
            var entityIds = stages.Select(n => n.EntityId).Distinct().ToList();
            int entityIndex = entityIds.FindIndex(n => n.Equals(currentStage.EntityId)) + 1;
            if (entityIndex == 1)
            {
                return this.Update(n => n
                        .Set(f => f.ProcessStageId, processStageId)
                        .Set(f => f.ProcessEntityId, entityId)
                        .Set(f => f.Entity1Id, recordId)
                        .Where(w => w.BusinessProcessFlowInstanceId == bpfInstance.BusinessProcessFlowInstanceId)
                    );
            }
            else if (entityIndex == 2)
            {
                return this.Update(n => n
                        .Set(f => f.ProcessStageId, processStageId)
                        .Set(f => f.ProcessEntityId, entityId)
                        .Set(f => f.Entity2Id, recordId)
                        .Where(w => w.BusinessProcessFlowInstanceId == bpfInstance.BusinessProcessFlowInstanceId)
                    );
            }
            else if (entityIndex == 3)
            {
                return this.Update(n => n
                        .Set(f => f.ProcessStageId, processStageId)
                        .Set(f => f.ProcessEntityId, entityId)
                        .Set(f => f.Entity3Id, recordId)
                        .Where(w => w.BusinessProcessFlowInstanceId == bpfInstance.BusinessProcessFlowInstanceId)
                    );
            }
            else if (entityIndex == 4)
            {
                return this.Update(n => n
                        .Set(f => f.ProcessStageId, processStageId)
                        .Set(f => f.ProcessEntityId, entityId)
                        .Set(f => f.Entity4Id, recordId)
                        .Where(w => w.BusinessProcessFlowInstanceId == bpfInstance.BusinessProcessFlowInstanceId)
                    );
            }
            else if (entityIndex == 5)
            {
                return this.Update(n => n
                        .Set(f => f.ProcessStageId, processStageId)
                        .Set(f => f.ProcessEntityId, entityId)
                        .Set(f => f.Entity5Id, recordId)
                        .Where(w => w.BusinessProcessFlowInstanceId == bpfInstance.BusinessProcessFlowInstanceId)
                    );
            }
            return false;
        }

        public void UpdateOnRecordDeleted(Guid entityId, Guid recordId)
        {
            var bpfInstances = _businessProcessFlowInstanceRepository.Query(f => (f.Entity1Id.IsNotNull() && f.Entity1Id == recordId)
            || (f.Entity2Id.IsNotNull() && f.Entity2Id == recordId)
            || (f.Entity3Id.IsNotNull() && f.Entity3Id == recordId)
            || (f.Entity4Id.IsNotNull() && f.Entity4Id == recordId)
            || (f.Entity5Id.IsNotNull() && f.Entity5Id == recordId)
            );
            foreach (var bpfInstance in bpfInstances)
            {
                var stages = _processStageService.Query(n => n
                   .Where(f => f.WorkFlowId == bpfInstance.WorkFlowId)
                   .Sort(s => s.SortAscending(f => f.StageOrder)));
                var entityIds = stages.Select(n => n.EntityId).Distinct().ToList();
                int entityIndex = entityIds.FindIndex(n => n.Equals(entityId)) + 1;
                if (entityIndex == 1)
                {
                    _businessProcessFlowInstanceRepository.DeleteById(bpfInstance.BusinessProcessFlowInstanceId);
                }
                //前一实体的最后一个阶段
                else
                {
                    var prevStage = stages.Where(n => n.EntityId == entityIds[entityIndex - 2]).ToList().Last();
                    UpdateBack(bpfInstance.WorkFlowId, bpfInstance.BusinessProcessFlowInstanceId, prevStage.ProcessStageId, recordId);
                }
            }
        }
    }
}
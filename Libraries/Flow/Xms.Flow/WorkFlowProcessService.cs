using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Core.Data;
using Xms.Flow.Data;
using Xms.Flow.Domain;
using Xms.Infrastructure.Utility;

namespace Xms.Flow
{
    /// <summary>
    /// 工作流执行步骤服务
    /// </summary>
    public class WorkFlowProcessService : IWorkFlowProcessService, ICascadeDelete<WorkFlowInstance>
    {
        private readonly IWorkFlowProcessRepository _workFlowProcessRepository;

        public WorkFlowProcessService(IWorkFlowProcessRepository workFlowProcessRepository)
        {
            _workFlowProcessRepository = workFlowProcessRepository;
        }

        /// <summary>
        /// 级联删除
        /// </summary>
        /// <param name="parent">被删除的审批流实例</param>
        public void CascadeDelete(params WorkFlowInstance[] parent)
        {
            if (parent.NotEmpty())
            {
                _workFlowProcessRepository.DeleteMany(x => x.WorkFlowInstanceId.In(parent.Select(f => f.WorkFlowInstanceId)));
            }
        }

        /// <summary>
        /// 新建一条记录
        /// </summary>
        /// <param name="entity">实体对象</param>
        /// <returns></returns>
        public bool Create(WorkFlowProcess entity)
        {
            return _workFlowProcessRepository.Create(entity);
        }

        /// <summary>
        /// 新建一批记录
        /// </summary>
        /// <param name="entities">实体列表</param>
        /// <returns></returns>
        public bool CreateMany(IEnumerable<WorkFlowProcess> entities)
        {
            return _workFlowProcessRepository.CreateMany(entities);
        }

        /// <summary>
        /// 删除一条记录
        /// </summary>
        /// <param name="id">记录主键值</param>
        /// <returns></returns>
        public bool DeleteById(Guid id)
        {
            return _workFlowProcessRepository.DeleteById(id);
        }

        /// <summary>
        /// 删除一批记录
        /// </summary>
        /// <param name="ids">记录主键值列表</param>
        /// <returns></returns>
        public bool DeleteById(IEnumerable<Guid> ids)
        {
            return _workFlowProcessRepository.DeleteMany(ids);
        }
    }
}
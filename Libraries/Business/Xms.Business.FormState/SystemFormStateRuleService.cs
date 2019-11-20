using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xms.Business.FormStateRule.Data;
using Xms.Business.FormStateRule.Domain;
using Xms.Core.Context;
using Xms.Data.Provider;

namespace Xms.Business.FormStateRule
{
    /// <summary>
    /// 表单状态服务
    /// </summary>
    public class SystemFormStateRuleService : ISystemFormStateRuleService
    {
        private readonly ISystemFormStateRuleRepository _systemFormStateRuleRepository;

        public SystemFormStateRuleService(ISystemFormStateRuleRepository systemFormStateRuleRepository)
        {
            _systemFormStateRuleRepository = systemFormStateRuleRepository;
        }

        public bool Create(SystemFormStateRule entity)
        {
            var flag = _systemFormStateRuleRepository.Create(entity);
            return flag;
        }

        public bool CreateMany(List<SystemFormStateRule> entities)
        {
            return _systemFormStateRuleRepository.CreateMany(entities);
        }

        public bool Update(SystemFormStateRule entity)
        {
            var flag = _systemFormStateRuleRepository.Update(entity);
            return flag;
        }

        public bool Update(Func<UpdateContext<SystemFormStateRule>, UpdateContext<SystemFormStateRule>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<SystemFormStateRule>());
            return _systemFormStateRuleRepository.Update(ctx);
        }

        public SystemFormStateRule FindById(Guid id)
        {
            var data = _systemFormStateRuleRepository.FindById(id);
            return data;
        }

        public SystemFormStateRule Find(Expression<Func<SystemFormStateRule, bool>> predicate)
        {
            var data = _systemFormStateRuleRepository.Find(predicate);
            return data;
        }

        public bool DeleteById(Guid id)
        {
            var flag = _systemFormStateRuleRepository.DeleteById(id);
            return flag;
        }

        public bool DeleteById(List<Guid> ids)
        {
            var flag = true;
            foreach (var id in ids)
            {
                flag = this.DeleteById(id);
            }
            return flag;
            //return _repository.DeleteById(ids);
        }

        public PagedList<SystemFormStateRule> QueryPaged(Func<QueryDescriptor<SystemFormStateRule>, QueryDescriptor<SystemFormStateRule>> container)
        {
            QueryDescriptor<SystemFormStateRule> q = container(QueryDescriptorBuilder.Build<SystemFormStateRule>());
            var datas = _systemFormStateRuleRepository.QueryPaged(q);
            return datas;
        }

        public PagedList<SystemFormStateRule> QueryPaged(Func<QueryDescriptor<SystemFormStateRule>, QueryDescriptor<SystemFormStateRule>> container, Guid solutionId, bool existInSolution)
        {
            QueryDescriptor<SystemFormStateRule> q = container(QueryDescriptorBuilder.Build<SystemFormStateRule>());
            var datas = _systemFormStateRuleRepository.QueryPaged(q, Module.Core.ModuleCollection.GetIdentity(FormStateRuleDefaults.ModuleName), solutionId, existInSolution);

            return datas;
        }

        public List<SystemFormStateRule> Query(Func<QueryDescriptor<SystemFormStateRule>, QueryDescriptor<SystemFormStateRule>> container)
        {
            QueryDescriptor<SystemFormStateRule> q = container(QueryDescriptorBuilder.Build<SystemFormStateRule>());
            var datas = _systemFormStateRuleRepository.Query(q)?.ToList();
            return datas;
        }
    }
}
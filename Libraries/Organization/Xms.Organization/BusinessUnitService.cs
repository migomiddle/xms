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
    /// 业务部门服务
    /// </summary>
    public class BusinessUnitService : IBusinessUnitService
    {
        private readonly IBusinessUnitRepository _businessUnitRepository;
        //private readonly ILocalizedTextProvider _loc;
        //private readonly ISystemUserService _systemUserService;

        public BusinessUnitService(//IWebAppContext appContext
            IBusinessUnitRepository businessUnitRepository
            //, ISystemUserService systemUserService
            )
        {
            //_loc = appContext.T;
            _businessUnitRepository = businessUnitRepository;
            //_systemUserService = systemUserService;
        }

        public bool Create(BusinessUnit entity)
        {
            return _businessUnitRepository.Create(entity);
        }

        public bool Update(BusinessUnit entity)
        {
            return _businessUnitRepository.Update(entity);
        }

        public bool Update(Func<UpdateContext<BusinessUnit>, UpdateContext<BusinessUnit>> context)
        {
            var ctx = context(UpdateContextBuilder.Build<BusinessUnit>());
            return _businessUnitRepository.Update(ctx);
        }

        public BusinessUnit FindById(Guid id)
        {
            return _businessUnitRepository.FindById(id);
        }

        public bool DeleteById(Guid id)
        {
            //检查部门下是否有用户
            //var hasUser = _systemUserService.Find(n => n.BusinessUnitId == id) != null;
            //if (hasUser)
            //{
            //    throw new XmsException(_loc["referenced"]);
            //}
            return _businessUnitRepository.DeleteById(id);
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

        public PagedList<BusinessUnit> QueryPaged(Func<QueryDescriptor<BusinessUnit>, QueryDescriptor<BusinessUnit>> container)
        {
            QueryDescriptor<BusinessUnit> q = container(QueryDescriptorBuilder.Build<BusinessUnit>());

            return _businessUnitRepository.QueryPaged(q);
        }

        public List<BusinessUnit> Query(Func<QueryDescriptor<BusinessUnit>, QueryDescriptor<BusinessUnit>> container)
        {
            QueryDescriptor<BusinessUnit> q = container(QueryDescriptorBuilder.Build<BusinessUnit>());

            return _businessUnitRepository.Query(q)?.ToList();
        }

        /// <summary>
        /// 获取所有下级部门
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public List<BusinessUnit> GetChilds(Guid parentId)
        {
            return _businessUnitRepository.GetChilds(parentId);
        }

        /// <summary>
        /// 是否子部门
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        public bool IsChild(Guid parentId, Guid businessUnitId)
        {
            return _businessUnitRepository.IsChild(parentId, businessUnitId);
        }
    }
}
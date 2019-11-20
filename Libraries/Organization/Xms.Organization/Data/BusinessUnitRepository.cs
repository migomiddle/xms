using PetaPoco;
using System;
using System.Collections.Generic;
using Xms.Core.Data;
using Xms.Data;
using Xms.Organization.Domain;

namespace Xms.Organization.Data
{
    /// <summary>
    /// 业务部门仓储
    /// </summary>
    public class BusinessUnitRepository : DefaultRepository<BusinessUnit>, IBusinessUnitRepository
    {
        public BusinessUnitRepository(IDbContext dbContext) : base(dbContext)
        {
        }

        #region implements

        /// <summary>
        /// 获取所有下级部门
        /// </summary>
        /// <param name="parentid"></param>
        /// <returns></returns>
        public List<BusinessUnit> GetChilds(Guid parentId)
        {
            return _repository.ExecuteQuery("SELECT BusinessUnitId FROM dbo.ufn_Org_GetDeptTree(@0)", parentId);
        }

        /// <summary>
        /// 是否子部门
        /// </summary>
        /// <param name="parentId"></param>
        /// <param name="businessUnitId"></param>
        /// <returns></returns>
        public bool IsChild(Guid parentId, Guid businessUnitId)
        {
            Sql s = Sql.Builder.Append("SELECT 1 FROM (SELECT BusinessUnitId FROM dbo.ufn_Org_GetDeptTree(@0)) AS B WHERE BusinessUnitId = @1", parentId, businessUnitId);
            return _repository.Exists(s);
        }

        #endregion implements
    }
}
namespace Xms.Core
{
    /// <summary>
    /// 实体操作权限级别
    /// </summary>
    public enum EntityPermissionDepth
    {
        /// <summary>
        /// 无
        /// </summary>
        None = 0,

        /// <summary>
        /// 本人
        /// </summary>
        Self = 1,

        /// <summary>
        /// 所在部门
        /// </summary>
        BusinessUnit = 2,

        /// <summary>
        /// 所在部门及下级部门
        /// </summary>
        BusinessUnitAndChild = 4,

        /// <summary>
        /// 所在组织
        /// </summary>
        Organization = 16
    }
}
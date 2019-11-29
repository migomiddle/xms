using System;

namespace Xms.Dependency
{
    /// <summary>
    /// 依赖项检测
    /// </summary>
    public interface IDependencyChecker
    {
        /// <summary>
        /// 检测并抛出异常(如果存在依赖)
        /// </summary>
        /// <typeparam name="TRequired">被依赖对象类型</typeparam>
        /// <param name="requiredComponentType">被依赖组件类型</param>
        /// <param name="requiredId">被依赖对象主键</param>
        void CheckAndThrow<TRequired>(int requiredComponentType, Guid requiredId);

        /// <summary>
        /// 检测并抛出异常(如果存在依赖)
        /// </summary>
        /// <typeparam name="TRequired">被依赖对象类型</typeparam>
        /// <param name="requiredComponentName">被依赖组件类型</param>
        /// <param name="requiredId">被依赖对象主键</param>
        void CheckAndThrow<TRequired>(string requiredComponentName, Guid requiredId);
    }
}
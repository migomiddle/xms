using System;
using Xms.Context;
using Xms.Dependency.Abstractions;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Module.Core;

namespace Xms.Dependency
{
    /// <summary>
    /// 依赖项检测
    /// </summary>
    public class DependencyChecker : IDependencyChecker
    {
        private readonly IDependencyLookupFactory _dependencyFinderFactory;
        private readonly IAppContext _appContext;

        public DependencyChecker(IAppContext appContext, IDependencyLookupFactory dependencyFinderFactory)
        {
            _appContext = appContext;
            _dependencyFinderFactory = dependencyFinderFactory;
        }

        /// <summary>
        /// 检测并抛出异常(如果存在依赖)
        /// </summary>
        /// <typeparam name="TRequired">被依赖对象类型</typeparam>
        /// <param name="requiredComponentType">被依赖组件类型</param>
        /// <param name="requiredId">被依赖对象主键</param>
        public void CheckAndThrow<TRequired>(int requiredComponentType, Guid requiredId)
        {
            var dependentComponents = _dependencyFinderFactory.GetDependents(requiredComponentType, requiredId);
            if (dependentComponents.NotEmpty())
            {
                throw new XmsDependencyException(_appContext.GetFeature<ILocalizedTextProvider>()["referenced"], dependentComponents);
            }
        }

        /// <summary>
        /// 检测并抛出异常(如果存在依赖)
        /// </summary>
        /// <typeparam name="TRequired">被依赖对象类型</typeparam>
        /// <param name="requiredComponentName">被依赖组件类型</param>
        /// <param name="requiredId">被依赖对象主键</param>
        public void CheckAndThrow<TRequired>(string requiredComponentName, Guid requiredId)
        {
            this.CheckAndThrow<TRequired>(ModuleCollection.GetIdentity(requiredComponentName), requiredId);
        }
    }
}
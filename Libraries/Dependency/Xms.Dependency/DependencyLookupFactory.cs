using System;
using System.Collections.Generic;
using System.Linq;
using Xms.Dependency.Abstractions;
using Xms.Infrastructure.Inject;
using Xms.Logging.AppLog;

namespace Xms.Dependency
{
    /// <summary>
    /// 依赖项查找工厂
    /// </summary>
    public class DependencyLookupFactory : IDependencyLookupFactory
    {
        private readonly IDependencyService _dependencyService;
        private readonly ILogService _logService;
        private readonly IServiceResolver _serviceResolver;

        public DependencyLookupFactory(IDependencyService dependencyService
            , ILogService logService
            , IServiceResolver serviceResolver)
        {
            _dependencyService = dependencyService;
            _logService = logService;
            _serviceResolver = serviceResolver;
        }

        #region Methods

        /// <summary>
        /// 获取依赖项清单
        /// </summary>
        /// <typeparam name="TRequired">被依赖的对象类型</typeparam>
        /// <param name="requiredComponentType">被依赖组件类型</param>
        /// <param name="requiredId">被依赖对象ID</param>
        /// <returns></returns>
        public virtual List<DependentDescriptor> GetDependents<TRequired>(int requiredComponentType, Guid requiredId)
        {
            List<DependentDescriptor> result = new List<DependentDescriptor>();
            try
            {
                var finders = _serviceResolver.GetAll<IDependentLookup<TRequired>>();
                var dependents = _dependencyService.Query(n => n.Where(f => (int)f.RequiredComponentType == requiredComponentType && f.RequiredObjectId == requiredId));
                foreach (var item in dependents)
                {
                    var finder = finders.FirstOrDefault(x => x.ComponentType == item.DependentComponentType);
                    if (finder != null)
                    {
                        var dependentDescriptor = finder.GetDependent(item.DependentObjectId);
                        if (dependentDescriptor != null)
                        {
                            dependentDescriptor.ComponentType = item.DependentComponentType;
                            dependentDescriptor.DependentId = item.DependentObjectId;
                            result.Add(dependentDescriptor);
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                try
                {
                    _logService.Error(exception);
                }
                catch { }
            }
            return result;
        }

        /// <summary>
        /// 获取依赖项清单
        /// </summary>
        /// <param name="requiredComponentType">被依赖组件类型</param>
        /// <param name="requiredId">被依赖对象ID</param>
        /// <returns></returns>
        public virtual List<DependentDescriptor> GetDependents(int requiredComponentType, Guid requiredId)
        {
            List<DependentDescriptor> result = new List<DependentDescriptor>();
            var finders = _serviceResolver.GetAll<IDependentLookup>();
            var dependents = _dependencyService.Query(n => n.Where(f => f.RequiredComponentType == requiredComponentType && f.RequiredObjectId == requiredId));
            foreach (var item in dependents)
            {
                var finder = finders.FirstOrDefault(x => x.ComponentType == item.DependentComponentType);
                if (finder != null)
                {
                    var dependentDescriptor = finder.GetDependent(item.DependentObjectId);
                    if (dependentDescriptor != null)
                    {
                        dependentDescriptor.ComponentType = item.DependentComponentType;
                        dependentDescriptor.DependentId = item.DependentObjectId;
                        result.Add(dependentDescriptor);
                    }
                }
            }
            return result;
        }

        public virtual List<DependentDescriptor> GetRequireds(int dependentComponentType, Guid dependentId)
        {
            List<DependentDescriptor> result = new List<DependentDescriptor>();
            var finders = _serviceResolver.GetAll<IDependentLookup>();
            var dependents = _dependencyService.Query(n => n.Where(f => f.DependentComponentType == dependentComponentType && f.DependentObjectId == dependentId));
            foreach (var item in dependents)
            {
                var finder = finders.FirstOrDefault(x => x.ComponentType == item.DependentComponentType);
                if (finder != null)
                {
                    var dependentDescriptor = finder.GetDependent(item.DependentObjectId);
                    if (dependentDescriptor != null)
                    {
                        dependentDescriptor.ComponentType = item.DependentComponentType;
                        dependentDescriptor.DependentId = item.DependentObjectId;
                        result.Add(dependentDescriptor);
                    }
                }
            }
            return result;
        }

        #endregion Methods
    }
}
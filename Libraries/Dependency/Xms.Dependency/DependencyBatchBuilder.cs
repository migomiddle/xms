using System;
using System.Collections.Generic;
using Xms.Infrastructure.Utility;
using Xms.Module.Core;

namespace Xms.Dependency
{
    /// <summary>
    /// 依赖项批量构建服务
    /// </summary>
    public class DependencyBatchBuilder : IDependencyBatchBuilder
    {
        private readonly IDependencyService _dependencyService;
        private readonly List<Domain.Dependency> _entities;

        public DependencyBatchBuilder(IDependencyService dependencyService)
        {
            _dependencyService = dependencyService;
            _entities = new List<Domain.Dependency>();
        }

        public virtual DependencyBatchBuilder Append(int dependentComponentType, Guid dependentObjectId, int requiredComponentType, params Guid[] requiredObjectId)
        {
            if (requiredObjectId.NotEmpty())
            {
                foreach (var item in requiredObjectId)
                {
                    var entity = new Domain.Dependency
                    {
                        DependentComponentType = dependentComponentType,
                        DependentObjectId = dependentObjectId,
                        RequiredComponentType = requiredComponentType,
                        RequiredObjectId = item
                    };
                    _entities.Add(entity);
                }
            }
            return this;
        }

        public virtual DependencyBatchBuilder Append(string dependentComponentName, Guid dependentObjectId, string requiredComponentName, params Guid[] requiredObjectId)
        {
            return this.Append(ModuleCollection.GetIdentity(dependentComponentName), dependentObjectId, ModuleCollection.GetIdentity(requiredComponentName), requiredObjectId);
        }

        public virtual bool Save()
        {
            if (_entities.NotEmpty())
            {
                var result = _dependencyService.CreateMany(_entities);
                if (result)
                {
                    Clear();
                }
                return result;
            }
            return false;
        }

        public virtual void Clear()
        {
            _entities.Clear();
        }
    }
}
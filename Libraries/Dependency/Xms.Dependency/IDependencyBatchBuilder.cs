using System;

namespace Xms.Dependency
{
    public interface IDependencyBatchBuilder
    {
        DependencyBatchBuilder Append(int dependentComponentType, Guid dependentObjectId, int requiredComponentType, params Guid[] requiredObjectId);

        DependencyBatchBuilder Append(string dependentComponentName, Guid dependentObjectId, string requiredComponentName, params Guid[] requiredObjectId);

        void Clear();

        bool Save();
    }
}
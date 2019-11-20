using System;

namespace Xms.Dependency.Abstractions
{
    /// <summary>
    /// 依赖项查找
    /// </summary>
    public interface IDependentLookup
    {
        DependentDescriptor GetDependent(Guid dependentId);

        int ComponentType { get; }
    }

    /// <summary>
    /// 依赖项查找
    /// </summary>
    /// <typeparam name="TRequired">被依赖对象，用于DI准确查找</typeparam>
    public interface IDependentLookup<TRequired> : IDependentLookup
    {
    }
}
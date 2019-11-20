using System;
using System.Collections.Generic;
using Xms.Dependency.Abstractions;

namespace Xms.Dependency
{
    /// <summary>
    /// 依赖项查找工厂
    /// </summary>
    public interface IDependencyLookupFactory
    {
        List<DependentDescriptor> GetDependents<TRequired>(int requiredComponentType, Guid requiredId);

        List<DependentDescriptor> GetDependents(int requiredComponentType, Guid requiredId);
    }
}
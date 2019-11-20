using System;
using Xms.Core.Data;

namespace Xms.Sdk.Client.AggRoot
{
    /// <summary>
    /// 聚合创建接口
    /// </summary>
    public interface IAggCreater
    {
        Guid Create(AggregateRoot aggregateRoot, Guid? SystemFormId, bool ignorePermissions = false);
    }
}

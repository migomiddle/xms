using System;
using Xms.Core.Data;

namespace Xms.Sdk.Client.AggRoot
{
    /// <summary>
    /// 聚合更新接口
    /// </summary>
    public interface IAggUpdater
    {
        bool Update(AggregateRoot aggregateRoot, Guid? SystemFormId, bool ignorePermissions = false);
    }
}
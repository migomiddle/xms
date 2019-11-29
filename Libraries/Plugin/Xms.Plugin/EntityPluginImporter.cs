using System;
using System.Collections.Generic;
using Xms.Context;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Plugin.Domain;
using Xms.Solution.Abstractions;

namespace Xms.Plugin
{
    /// <summary>
    /// 实体插件导入服务
    /// </summary>
    [SolutionImportNode("entityplugins")]
    public class EntityPluginImporter : ISolutionComponentImporter<EntityPlugin>
    {
        private readonly IEntityPluginCreater _entityPluginCreater;
        private readonly IEntityPluginUpdater _entityPluginUpdater;
        private readonly IEntityPluginFinder _entityPluginFinder;
        private readonly IAppContext _appContext;

        public EntityPluginImporter(IAppContext appContext
            , IEntityPluginCreater entityPluginCreater
            , IEntityPluginUpdater entityPluginUpdater
            , IEntityPluginFinder entityPluginFinder)
        {
            _appContext = appContext;
            _entityPluginCreater = entityPluginCreater;
            _entityPluginUpdater = entityPluginUpdater;
            _entityPluginFinder = entityPluginFinder;
        }

        public bool Import(Guid solutionId, IList<EntityPlugin> entityPlugins)
        {
            if (entityPlugins.NotEmpty())
            {
                foreach (var item in entityPlugins)
                {
                    var entity = _entityPluginFinder.FindById(item.EntityPluginId);
                    if (entity != null)
                    {
                        entity.AssemblyName = item.AssemblyName;
                        entity.ClassName = item.ClassName;
                        entity.EventName = item.EventName;
                        entity.ProcessOrder = item.ProcessOrder;
                        entity.TypeCode = item.TypeCode;
                        _entityPluginUpdater.Update(entity);
                    }
                    else
                    {
                        item.SolutionId = solutionId;
                        item.ComponentState = 0;
                        item.CreatedBy = _appContext.GetFeature<ICurrentUser>().SystemUserId;
                        item.CreatedOn = DateTime.Now;
                        _entityPluginCreater.Create(item);
                    }
                }
            }
            return true;
        }
    }
}
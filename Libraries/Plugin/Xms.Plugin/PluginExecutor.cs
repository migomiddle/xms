using System;
using Xms.Context;
using Xms.Core;
using Xms.Identity;
using Xms.Infrastructure.Inject;
using Xms.Infrastructure.Utility;
using Xms.Plugin.Abstractions;
using Xms.Plugin.Domain;

namespace Xms.Plugin
{
    /// <summary>
    /// 实体插件执行器
    /// </summary>
    public class PluginExecutor<TData, KMetadata> : IPluginExecutor<TData, KMetadata>
    {
        private readonly IEntityPluginFinder _entityPluginFinder;
        private readonly IEntityPluginFileProvider _entityPluginFileProvider;
        private readonly IAppContext _appContext;
        private readonly IServiceResolver _serviceResolver;
        private readonly ICurrentUser _currentUser;

        public PluginExecutor(IAppContext appContext
            , IEntityPluginFinder entityPluginFinder
            , IEntityPluginFileProvider entityPluginFileProvider
            , IServiceResolver serviceResolver)
        {
            _appContext = appContext;
            _currentUser = _appContext.GetFeature<ICurrentUser>();
            _entityPluginFinder = entityPluginFinder;
            _entityPluginFileProvider = entityPluginFileProvider;
            _serviceResolver = serviceResolver;
        }

        public void Execute(Guid entityId, Guid? businessObjectId, PlugInType typeCode, OperationTypeEnum op, OperationStage stage, TData tData, KMetadata kMetadata)
        {
            var plugins = _entityPluginFinder.QueryByEntityId(entityId, Enum.GetName(typeof(OperationTypeEnum), op), businessObjectId, typeCode);

            if (plugins.NotEmpty())
            {
                foreach (var pg in plugins)
                {
                    if (pg.StateCode == RecordState.Disabled)
                    {
                        continue;
                    }
                    var pinstance = GetInstance(pg);
                    if (pinstance != null)
                    {
                        pinstance.Execute(new PluginExecutionContextT<TData, KMetadata>()
                        {
                            MessageName = op
                            ,
                            Stage = stage
                            ,
                            User = _currentUser
                            ,
                            Target = tData
                            ,
                            metadata = kMetadata
                        });
                    }
                }
            }
        }

        public IPlugin<TData, KMetadata> GetInstance(EntityPlugin entity)
        {
            IPlugin<TData, KMetadata> _instance = null;
            if (_entityPluginFileProvider.LoadAssembly(entity))
            {
                _instance = (IPlugin<TData, KMetadata>)_serviceResolver.ResolveUnregistered(Type.GetType(entity.ClassName, false, true));
            }
            return _instance;
        }
    }
}
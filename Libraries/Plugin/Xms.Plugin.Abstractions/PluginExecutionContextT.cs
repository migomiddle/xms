using Xms.Core;
using Xms.Core.Context;

namespace Xms.Plugin.Abstractions
{
    public class PluginExecutionContextT<TData, KMetadata>
    {
        public OperationTypeEnum MessageName { get; set; }
        public OperationStage Stage { get; set; }
        public IUserContext User { get; set; }

        public TData Target { get; set; }
        public KMetadata metadata { get; set; }
    }
}
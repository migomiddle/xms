namespace Xms.Plugin.Abstractions
{
    public interface IPlugin<TData, KMetadata> : IAbsPlugin
    {
        void Execute(PluginExecutionContextT<TData, KMetadata> context);
    }
}
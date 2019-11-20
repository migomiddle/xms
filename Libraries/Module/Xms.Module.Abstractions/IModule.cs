using System;

namespace Xms.Module.Abstractions
{
    public interface IModule
    {
        //string Name { get; }

        Action<ModuleDescriptor> Configure();

        void OnStarting();
    }
}
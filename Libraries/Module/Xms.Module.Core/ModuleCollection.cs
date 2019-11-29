using System;
using System.Collections.Concurrent;
using Xms.Module.Abstractions;

namespace Xms.Module.Core
{
    public class ModuleCollection
    {
        public static readonly ConcurrentDictionary<string, ModuleDescriptor> Descriptors = new ConcurrentDictionary<string, ModuleDescriptor>(StringComparer.InvariantCultureIgnoreCase);

        public static void Configure(Action<ModuleDescriptor> setupAction)
        {
            if (setupAction != null)
            {
                ModuleDescriptor descriptor = new ModuleDescriptor();
                setupAction(descriptor);
                if (Descriptors.ContainsKey(descriptor.Name))
                {
                    throw new Exception($"name '{descriptor.Name}' is already exists");
                }
                Descriptors.TryAdd(descriptor.Name, descriptor);
            }
        }

        public static ModuleDescriptor GetDescriptor(string name)
        {
            if (Descriptors.TryGetValue(name, out ModuleDescriptor descriptor))
            {
                return descriptor;
            }
            throw new Exception($"name '{name}' does not exists");
        }

        public static int GetIdentity(string name)
        {
            if (Descriptors.TryGetValue(name, out ModuleDescriptor descriptor))
            {
                return descriptor.Identity;
            }
            throw new Exception($"name '{name}' does not exists");
        }
    }
}
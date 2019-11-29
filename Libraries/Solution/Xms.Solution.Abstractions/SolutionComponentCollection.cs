using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Xms.Infrastructure;

namespace Xms.Solution.Abstractions
{
    public class SolutionComponentCollection
    {
        public static readonly ConcurrentDictionary<string, SolutionComponentDescriptor> Descriptors = new ConcurrentDictionary<string, SolutionComponentDescriptor>(StringComparer.InvariantCultureIgnoreCase);

        public static void Configure(Action<SolutionComponentDescriptor> setupAction)
        {
            if (setupAction != null)
            {
                SolutionComponentDescriptor componentDescriptor = new SolutionComponentDescriptor();
                setupAction(componentDescriptor);
                if (Descriptors.ContainsKey(componentDescriptor.Module.Name))
                {
                    throw new XmsException($"name '{componentDescriptor.Module.Name}' is already exists");
                }
                Descriptors.TryAdd(componentDescriptor.Module.Name, componentDescriptor);
            }
        }

        public static SolutionComponentDescriptor GetDescriptor(string name)
        {
            if (Descriptors.TryGetValue(name, out SolutionComponentDescriptor descriptor))
            {
                return descriptor;
            }
            throw new XmsException($"name '{name}' does not exists");
        }

        public static List<SolutionComponentDescriptor> SortedDescriptors
        {
            get
            {
                return Descriptors.Values.OrderBy(x => x.Module.Identity).ToList();
            }
        }
    }
}
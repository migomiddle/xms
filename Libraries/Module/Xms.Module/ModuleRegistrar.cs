using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using Xms.Infrastructure.Utility;
using Xms.Module.Abstractions;
using Xms.Module.Core;

namespace Xms.Module
{
    /// <summary>
    /// 模块扫描注册器
    /// </summary>
    public class ModuleRegistrar : IModuleRegistrar
    {
        private readonly IModuleService _moduleService;

        public ModuleRegistrar(IModuleService moduleService)
        {
            _moduleService = moduleService;
        }

        public void RegisterAll()
        {
            var registeredDescriptors = _moduleService.Query(x => x.Sort(s => s.SortAscending(f => f.Identity)));
            var implementTypes = AssemblyHelper.GetClassOfType(typeof(IModule));
            int nextIdentity = registeredDescriptors.NotEmpty() ? registeredDescriptors.Last().Identity : 0;
            foreach (var impl in implementTypes)
            {
                var instance = (IModule)Activator.CreateInstance(impl);
                var descriptor = new ModuleDescriptor();
                instance.Configure().Invoke(descriptor);
                var registeredDescriptor = registeredDescriptors.Find(x => x.Name.IsCaseInsensitiveEqual(descriptor.Name));
                if (registeredDescriptor != null)
                {
                    descriptor.Identity = registeredDescriptor.Identity;
                    descriptor.LocalizedName = registeredDescriptor.LocalizedName;
                }
                else
                {
                    _moduleService.Create(new Domain.Module
                    {
                        Identity = nextIdentity
                        ,
                        Name = descriptor.Name
                        ,
                        EntryClassName = impl.FullName
                        ,
                        LocalizedName = descriptor.LocalizedName
                    });
                }
                ModuleCollection.Configure((o) =>
                {
                    o.Identity = descriptor.Identity;
                    o.Name = descriptor.Name;
                    o.LocalizedName = descriptor.LocalizedName;
                });
                instance.OnStarting();
                nextIdentity++;
            }
        }
    }

    public static class ModuleRegistrarServiceExtensions
    {
        public static void RegisterModules(this IServiceCollection services)
        {
            using (var serviceProvider = services.BuildServiceProvider())
            {
                serviceProvider.GetService<IModuleRegistrar>().RegisterAll();
            }
        }
    }
}
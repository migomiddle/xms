using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xms.Infrastructure.Inject;
using Xms.Infrastructure.Utility;
using Xms.Solution.Abstractions;

namespace Xms.Solution
{
    /// <summary>
    /// 解决方案服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Solution.ISolutionService, Solution.SolutionService>();
            services.AddScoped<Solution.ISolutionComponentService, Solution.SolutionComponentService>();
            services.AddScoped<Solution.ISolutionImporter, Solution.SolutionImporter>();
            services.AddScoped<Solution.ISolutionExporter, Solution.SolutionExporter>();

            //solution exporter
            var solutionComponentExporters = AssemblyHelper.GetClassOfType(typeof(Solution.Abstractions.ISolutionComponentExporter), "Xms.*.dll");
            foreach (var exporter in solutionComponentExporters)
            {
                services.AddScoped(typeof(Solution.Abstractions.ISolutionComponentExporter), exporter);
            }
            //solution importer
            var solutionComponentImporters = AssemblyHelper.GetClassOfType(typeof(Solution.Abstractions.ISolutionComponentImporter<>), "Xms.*.dll");
            foreach (var importer in solutionComponentImporters)
            {
                var it = importer.FindInterfaces((type, criteria) =>
                {
                    var isMatch = type.IsGenericType && ((Type)criteria).IsAssignableFrom(type.GetGenericTypeDefinition());
                    return isMatch;
                }, typeof(Solution.Abstractions.ISolutionComponentImporter<>));
                foreach (var i in it)
                {
                    services.AddScoped(i, importer);
                    var nodeAttributes = importer.GetCustomAttributes(typeof(SolutionImportNodeAttribute), true);
                    if (nodeAttributes != null && nodeAttributes.Length > 0)
                    {
                        ImporterNodeTypeMapper.Add(((SolutionImportNodeAttribute)nodeAttributes[0]).Name, importer);
                    }
                    else
                    {
                        throw new Exception($"class '{importer.Name}' not assigned 'SolutionImportNodeAttribute'");
                    }
                }
            }
        }

        public int Order => 1;
    }
}
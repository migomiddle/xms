using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Xms.Infrastructure.Inject;

namespace Xms.Flow
{
    /// <summary>
    /// 流程服务注册
    /// </summary>
    public class ServiceRegistrar : IServiceRegistrar
    {
        public int Order => 1;

        public void Add(IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<Flow.IWorkFlowFinder, Flow.WorkFlowFinder>();
            services.AddScoped<Flow.IWorkFlowCreater, Flow.WorkFlowCreater>();
            services.AddScoped<Flow.IWorkFlowUpdater, Flow.WorkFlowUpdater>();
            services.AddScoped<Flow.IWorkFlowDeleter, Flow.WorkFlowDeleter>();
            services.AddScoped<Flow.IWorkFlowInstanceService, Flow.WorkFlowInstanceService>();
            services.AddScoped<Flow.IWorkFlowProcessService, Flow.WorkFlowProcessService>();
            services.AddScoped<Flow.IWorkFlowProcessUpdater, Flow.WorkFlowProcessUpdater>();
            services.AddScoped<Flow.IWorkFlowProcessFinder, Flow.WorkFlowProcessFinder>();
            services.AddScoped<Flow.IWorkFlowHandlerFinder, Flow.WorkFlowHandlerFinder>();
            services.AddScoped<Flow.IWorkFlowStepService, Flow.WorkFlowStepService>();
            services.AddScoped<Flow.IWorkFlowProcessLogService, Flow.WorkFlowProcessLogService>();
            services.AddScoped<Flow.IBusinessProcessFlowInstanceService, Flow.BusinessProcessFlowInstanceService>();
            services.AddScoped<Flow.IBusinessProcessFlowInstanceUpdater, Flow.BusinessProcessFlowInstanceUpdater>();
            services.AddScoped<Flow.IProcessStageService, Flow.ProcessStageService>();
            services.AddScoped<Flow.IWorkFlowExecuter, Flow.WorkFlowExecuter>();
            services.AddScoped<Flow.IWorkFlowStarter, Flow.WorkFlowStarter>();
            services.AddScoped<Flow.IWorkFlowCanceller, Flow.WorkFlowCanceller>();
            services.AddScoped<Flow.IWorkFlowDependency, Flow.WorkFlowDependency>();
        }
    }
}
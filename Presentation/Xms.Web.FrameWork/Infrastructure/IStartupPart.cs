using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Xms.Web.Framework.Infrastructure
{
    /// <summary>
    /// 子模块启动部分
    /// </summary>
    public interface IStartupPart
    {
        void ConfigureServices(IServiceCollection services);

        void Configure(IApplicationBuilder app);
    }
}
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Xms.Infrastructure.Inject
{
    /// <summary>
    /// 服务自动注册接口
    /// </summary>
    public interface IServiceRegistrar
    {
        void Add(IServiceCollection services, IConfiguration configuration);

        int Order { get; }
    }
}
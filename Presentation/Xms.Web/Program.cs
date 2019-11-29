using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using System;

namespace Xms.Web
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            var configuration = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
                                                  .AddJsonFile("appsettings.json")
                                                  .Build();
            return WebHost.CreateDefaultBuilder(args).UseUrls(configuration["urls"])
                .UseStartup<Startup>();
        }
    }
}
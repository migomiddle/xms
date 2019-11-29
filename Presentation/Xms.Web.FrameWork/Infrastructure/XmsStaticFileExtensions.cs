using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.FileProviders;
using System.Collections.Generic;
using System.IO;

namespace Xms.Web.Framework.Infrastructure
{
    /// <summary>
    /// 静态文件配置
    /// </summary>
    public static class XmsStaticFileExtensions
    {
        public static IApplicationBuilder UseXmsStaticFiles(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            app.UseStaticFiles();
            var staticFiles = configuration.GetSection("webconfig:staticfiles").Get<List<StaticFileItem>>();
            if (staticFiles != null)
            {
                foreach (var sf in staticFiles)
                {
                    var path = Path.Combine(env.ContentRootPath, sf.FilePath);
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    app.UseStaticFiles(new StaticFileOptions
                    {
                        FileProvider = new PhysicalFileProvider(path),
                        RequestPath = sf.RequestPath
                    });
                }
            }
            return app;
        }
    }

    public class StaticFileItem
    {
        public string FilePath { get; set; }
        public string RequestPath { get; set; }
    }
}
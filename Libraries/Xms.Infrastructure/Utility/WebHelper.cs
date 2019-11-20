using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading;

namespace Xms.Infrastructure.Utility
{
    public class WebHelper : IWebHelper
    {
        private static readonly Regex s_htmlPathPattern = new Regex(@"(?<=(?:href|src)=(?:""|'))(?!https?://)(?<url>[^(?:""|')]+)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);
        private static readonly Regex s_cssPathPattern = new Regex(@"url\('(?<url>.+)'\)", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.Multiline);

        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly HttpContext _httpContext;

        public WebHelper(IHostingEnvironment hostingEnvironment, IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _httpContext = httpContextAccessor?.HttpContext;
        }

        /// <summary>
        /// 获取文件绝对路径
        /// </summary>
        /// <param name="path">虚拟路径</param>
        /// <param name="autoCreate">不存在是否自动创建？</param>
        /// <returns></returns>
        public string MapPath(string path, bool autoCreate = false)
        {
            Guard.NotNull(() => path);

            string baseDirectory = _hostingEnvironment.ContentRootPath;
            path = path.Replace("~/", "").TrimStart('/').Replace('\\', '/');//linux下文件夹路径要用左斜杠

            var destPath = Path.Combine(baseDirectory, path);
            if (autoCreate)
            {
                if (!Directory.Exists(destPath))
                {
                    Directory.CreateDirectory(destPath);
                }
            }
            return destPath;
        }

        public void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "", bool aggressive = false)
        {
            //XmsApplication.AppBuilder.New();
            if (aggressive)
            {
                TryWriteBinFolder();
            }
            else
            {
                // without this, MVC may fail resolving controllers for newly installed plugins after IIS restart
                Thread.Sleep(250);
            }

            // If setting up plugins requires an AppDomain restart, it's very unlikely the
            // current request can be processed correctly.  So, we redirect to the same URL, so that the
            // new request will come to the newly started AppDomain.
            if (makeRedirect)
            {
                if (_httpContext.IsGetRequest())
                {
                    _httpContext.Response.Redirect(redirectUrl.IfEmpty("/"), true /*endResponse*/);
                }
                else
                {
                    // Don't redirect posts...
                    _httpContext.Response.ContentType = "text/html";
                    _httpContext.Response.SendFileAsync("~/refresh.html").Wait();
                    _httpContext.Response.AppendTrailer("", "");
                }
            }
        }

        private bool TryWriteWebConfig()
        {
            try
            {
                // In medium trust, "UnloadAppDomain" is not supported. Touch web.config
                // to force an AppDomain restart.
                File.SetLastWriteTimeUtc(MapPath("~/appsettings.json"), DateTime.UtcNow);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private bool TryWriteBinFolder()
        {
            try
            {
                var binMarker = Path.Combine(AppContext.BaseDirectory, "/bin/HostRestart");
                Directory.CreateDirectory(binMarker);

                using (var stream = File.CreateText(Path.Combine(binMarker, "marker.txt")))
                {
                    stream.WriteLine("Restart on '{0}'", DateTime.UtcNow);
                    stream.Flush();
                }
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Prepends protocol and host to all (relative) urls in a html string
        /// </summary>
        /// <param name="html">The html string</param>
        /// <param name="protocol">The protocol to prepend, e.g. <c>http</c></param>
        /// <param name="host">The host name to prepend, e.g. <c>www.mysite.com</c></param>
        /// <returns>The transformed result html</returns>
        /// <remarks>
        /// All html attributed named <c>src</c> and <c>href</c> are affected, also occurences of <c>url('path')</c> within embedded stylesheets.
        /// </remarks>
        public string MakeAllUrlsAbsolute(string html, string protocol, string host)
        {
            Guard.NotEmpty(() => html);
            Guard.NotEmpty(() => protocol);
            Guard.NotEmpty(() => host);

            string baseUrl = string.Format("{0}://{1}", protocol, host.TrimEnd('/'));

            MatchEvaluator evaluator = (match) =>
            {
                var url = match.Groups["url"].Value;
                return "{0}{1}".FormatCurrent(baseUrl, url.EnsureStartsWith("/"));
            };

            html = s_htmlPathPattern.Replace(html, evaluator);
            html = s_cssPathPattern.Replace(html, evaluator);

            return html;
        }
    }
}
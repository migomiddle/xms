using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Primitives;
using System.Text.RegularExpressions;

namespace Xms.Infrastructure.Utility
{
    public static class HttpContextExtensions
    {
        private static readonly Regex s_staticExts = new Regex(@"(.*?)\.(css|js|png|jpg|jpeg|gif|bmp|html|htm|xml|pdf|doc|xls|rar|zip|ico|eot|svg|ttf|woff|otf|axd|ashx|less)", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        /// <summary>
        /// 是否Ajax请求
        /// </summary>
        /// <returns></returns>
        public static bool IsAjaxRequest(this HttpContext context)
        {
            if (context.Request != null && context.Request.Headers["X-Requested-With"] != StringValues.Empty)
            {
                return context.Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            }
            return false;
        }

        /// <summary>
        /// 是否请求JSON数据
        /// </summary>
        /// <returns></returns>
        public static bool IsRequestJson(this HttpContext context)
        {
            if (context.Request != null)// && context.Request.Headers["Accept"] != StringValues.Empty
            {
                return context.Request.ContentType?.IndexOf("application/json") >= 0 || context.Request.Headers["Accept"].ToString().IndexOf("application/json") >= 0;
            }
            return false;
        }

        /// <summary>
        /// 获取当前访问url
        /// </summary>
        /// <param name="context"></param>
        /// <param name="includeQueryString"></param>
        /// <returns></returns>
        public static string GetThisPageUrl(this HttpContext context, bool includeQueryString = true)
        {
            string url = string.Empty;
            if (context.Request == null)
            {
                return url;
            }

            url = context.Request.Path.Value;
            if (includeQueryString)
            {
                url += context.Request.QueryString.Value;
            }

            return url.ToLowerInvariant();
        }

        /// <summary>
        /// 获取客户端IP
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetClientIpAddress(this HttpContext context)
        {
            string result = null;

            if (context.Request != null)
            {
                result = context.Request.Host.Value;
            }

            if (result == "::1")
            {
                result = "127.0.0.1";
            }

            return result.EmptyNull();
        }

        /// <summary>
        /// 获取上一个请求的地址
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetUrlReferrer(this HttpContext context)
        {
            string referrerUrl = string.Empty;

            if (context.Request != null &&
                context.Request.Headers["Referer"] != StringValues.Empty)
            {
                referrerUrl = context.Request.Headers["Referer"].ToString();
            }

            return referrerUrl;
        }

        /// <summary>
        /// 是否get请求
        /// </summary>
        /// <returns></returns>
        public static bool IsGetRequest(this HttpContext context)
        {
            if (context.Request != null)
            {
                return context.Request.Method == "GET";
            }
            return false;
        }

        /// <summary>
        /// 是否post请求
        /// </summary>
        /// <returns></returns>
        public static bool IsPostRequest(this HttpContext context)
        {
            if (context.Request != null)
            {
                return context.Request.Method == "POST";
            }
            return false;
        }

        public static bool IsStaticResourceRequested(this HttpContext context)
        {
            return s_staticExts.IsMatch(context.Request.Path);
        }

        #region 获取请求参数

        /// <summary>
        /// 获取URL中的值
        /// </summary>
        /// <param name="key"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static string GetQueryString(this HttpContext context, string key, string defaultValue = null)
        {
            var q = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(context.Request.QueryString.ToString());
            if (q != null && q.ContainsKey(key))
            {
                return q[key];
            }
            return defaultValue;
        }

        /// <summary>
        /// 获得路由中的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static string GetRouteString(this HttpContext context, string key, string defaultValue)
        {
            object value = context.GetRouteValue(key);
            if (value != null)
            {
                return value.ToString();
            }
            else
            {
                return defaultValue;
            }
        }

        /// <summary>
        /// 获得路由中的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetRouteString(this HttpContext context, string key)
        {
            return GetRouteString(context, key, "");
        }

        /// <summary>
        /// 获得路由中的值或URL参数的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static string GetRouteOrQueryString(this HttpContext context, string key)
        {
            var result = GetRouteString(context, key, null);
            if (result == null)
            {
                result = GetQueryString(context, key, null);
            }
            return result;
        }

        /// <summary>
        /// 获得路由中的值
        /// </summary>
        /// <param name="key">键</param>
        /// <param name="defaultValue">默认值</param>
        /// <returns></returns>
        public static int GetRouteInt(this HttpContext context, string key, int defaultValue)
        {
            int result = defaultValue;
            if (context.GetRouteValue(key) != null)
            {
                int.TryParse(context.GetRouteValue(key).ToString(), out result);
            }
            return result;
        }

        /// <summary>
        /// 获得路由中的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static int GetRouteInt(this HttpContext context, string key)
        {
            return GetRouteInt(context, key, 0);
        }

        /// <summary>
        /// 获得路由中的值或URL参数的值
        /// </summary>
        /// <param name="key">键</param>
        /// <returns></returns>
        public static int GetRouteOrQueryInt(this HttpContext context, string key)
        {
            return GetRouteInt(context, key) > 0 ? GetRouteInt(context, key) : (!GetQueryString(context, key).IsEmpty() ? int.Parse(GetQueryString(context, key)) : -1);
        }

        #endregion 获取请求参数
    }
}
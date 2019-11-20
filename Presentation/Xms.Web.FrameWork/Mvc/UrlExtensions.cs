using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Xms.Infrastructure.Utility;

namespace Xms.Web.Framework
{
    public static class UrlExtensions
    {
        /// <summary>
        /// 生成链接地址
        /// </summary>
        public static string ActionUrl(this IUrlHelper helper, ViewContext _viewcontext, string _actionName = "", params object[] _routeValues)
        {
            RouteValueDictionary _routed = _viewcontext.RouteData.Values;//路由值集合
            var queryString = _viewcontext.HttpContext.Request.Query;
            foreach (string key in queryString.Keys)
            {
                if (key != null && !_routed.ContainsKey(key))
                    _routed.Add(key, queryString[key]);
            }
            _actionName = _actionName.IfEmpty(_viewcontext.RouteData.Values["action"].ToString());

            return helper.Action(_actionName, _routed);
        }
    }
}
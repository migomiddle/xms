using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.AspNetCore.Routing.Template;

namespace Xms.Web.Framework.Route
{
    /// <summary>
    /// 组织路由匹配
    /// </summary>
    public class OrgRouteMatcher
    {
        private const string ORG_ROUTETEMPLATE = "{org}/{controller}/{action}";///{area:exists}

        public static RouteValueDictionary Match(HttpContext context)
        {
            var template = TemplateParser.Parse(ORG_ROUTETEMPLATE);
            var matcher = new TemplateMatcher(template, GetDefaults(template));
            var routeData = new RouteValueDictionary();
            matcher.TryMatch(context.Request.Path.Value, routeData);
            return routeData;
        }

        private static RouteValueDictionary GetDefaults(RouteTemplate parsedTemplate)
        {
            var result = new RouteValueDictionary();
            foreach (var parameter in parsedTemplate.Parameters)
            {
                if (parameter.DefaultValue != null)
                {
                    result.Add(parameter.Name, parameter.DefaultValue);
                }
            }
            return result;
        }
    }
}
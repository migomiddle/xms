using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Text;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Web.Framework.Context;

namespace Xms.Web.Framework.Controller
{
    public class XmsControllerBase : Microsoft.AspNetCore.Mvc.Controller
    {
        #region fields

        //上下文
        public IWebAppContext WebContext;

        public bool IsAjaxRequest
        {
            get
            {
                return HttpContext.IsAjaxRequest();
            }
        }

        public bool IsRequestJson
        {
            get
            {
                return HttpContext.IsRequestJson();
            }
        }

        public ICurrentUser CurrentUser
        {
            get
            {
                if (WebContext.CurrentUser != null)
                {
                    return WebContext.CurrentUser;
                }
                return null;
            }
        }

        protected ILocalizedTextProvider T
        {
            get
            {
                return WebContext.T;
            }
        }

        #endregion fields

        protected XmsControllerBase(IWebAppContext appContext)
        {
            WebContext = appContext;
        }

        public override void OnActionExecuting(ActionExecutingContext executingContext)
        {
            base.OnActionExecuting(executingContext);
            var httpContext = executingContext.HttpContext;
            WebContext.IsAjaxRequest = httpContext.IsAjaxRequest();
            WebContext.IP = httpContext.GetClientIpAddress();
            WebContext.Url = httpContext.GetThisPageUrl(includeQueryString: true);
            WebContext.UrlReferrer = httpContext.GetUrlReferrer();
            WebContext.Area = httpContext.GetRouteValue("area")?.ToString();//.ToLower();
            WebContext.ControllerName = httpContext.GetRouteValue("controller").ToString();//.ToLower();
            WebContext.ActionName = httpContext.GetRouteValue("action").ToString();//.ToLower();
            WebContext.Theme = httpContext.Request.Cookies["theme"].ToSafe("default");
        }

        /// <summary>
        /// 获取模型绑定错误信息
        /// </summary>
        /// <param name="state"></param>
        /// <returns></returns>
        protected string GetModelErrors(ModelStateDictionary state = null, bool jsonFormat = false)
        {
            StringBuilder msg = new StringBuilder();
            state = state ?? ModelState;
            if (state.IsValid)
            {
                return string.Empty;
            }
            var validationErrors = state.Where(x => x.Value.ValidationState == ModelValidationState.Invalid)
                .Select(x => new { key = x.Key, errors = string.Join('\n', x.Value.Errors.Select(e => e.ErrorMessage).ToArray()) });
            if (jsonFormat)
            {
                return validationErrors.SerializeToJson();
            }
            return string.Join('\n', validationErrors);
            //foreach (var item in state.Values)
            //{
            //    if (item.Errors.Count > 0)
            //    {
            //        if (item.Errors[0].Exception != null)
            //        {
            //            msg.Append(item.Errors[0].Exception.Message + "\n");
            //        }
            //        else
            //        {
            //            msg.Append(item.Errors[0].ErrorMessage + "\n");
            //        }
            //    }
            //}
            //return msg.ToString();
        }
    }
}
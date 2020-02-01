using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using System.Linq;
using System.Text;
using Xms.Identity;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Mvc;

namespace Xms.Api.Core.Controller
{
    /// <summary>
    /// 接口控制器基类
    /// </summary>
    //[TypeFilter(typeof(IdentityFilterAttribute), Order = 1)]
    [Authorize]
    [ApiController]
    [EnableCors("CorsPolicy")]//跨域
    public class ApiControllerBase : Microsoft.AspNetCore.Mvc.Controller
    {
        #region fields
        //上下文
        public IWebAppContext WebContext;

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
        #endregion

        protected ApiControllerBase(IWebAppContext appContext)
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
            var validationErrors = state
                .Keys
                .SelectMany(k => state[k].Errors)
                .Select(e => e.ErrorMessage)
                .ToArray();
            if (jsonFormat)
            {
                return validationErrors.SerializeToJson();
            }
            return string.Join('\n', validationErrors);
        }

        #region 常用返回信息
        protected IActionResult JError(object content, object extra = null)
        {
            return JResult.Error(content, extra);
        }
        protected IActionResult JOk(object content, object extra = null)
        {
            return JResult.Ok(content, extra);
        }
        protected IActionResult JError(string content, object extra = null)
        {
            return JResult.Error(content, extra);
        }
        protected IActionResult JOk(string content, object extra = null)
        {
            return JResult.Ok(content, extra);
        }
        protected IActionResult CreateFailure(string appendMsg = "", object extra = null)
        {
            return JResult.Error(T["created_error"] + ":" + appendMsg, extra);
        }
        protected IActionResult CreateSuccess(object extra = null)
        {
            return JResult.Ok(T["created_success"], extra);
        }
        protected IActionResult UpdateFailure(string appendMsg = "", object extra = null)
        {
            return JResult.Error(T["updated_error"] + ":" + appendMsg, extra);
        }
        protected IActionResult UpdateSuccess(object extra = null)
        {
            return JResult.Ok(T["updated_success"], extra);
        }
        protected IActionResult DeleteFailure(string appendMsg = "", object extra = null)
        {
            return JResult.Error(T["deleted_error"] + ":" + appendMsg, extra);
        }
        protected IActionResult DeleteSuccess(object extra = null)
        {
            return JResult.Ok(T["deleted_success"], extra);
        }
        protected IActionResult SaveFailure(string appendMsg = "", object extra = null)
        {
            return JResult.Error(T["saved_error"] + ":" + appendMsg, extra);
        }
        protected IActionResult SaveSuccess(object extra = null)
        {
            return JResult.Ok(T["saved_success"], extra);
        }
        protected IActionResult NotSpecifiedRecord(object extra = null)
        {
            return JResult.Error(T["notspecified_record"], extra);
        }
        protected IActionResult JModelError(string title = "")
        {
            return JResult.Error((title.IsNotEmpty() ? title + ": " : "") + GetModelErrors());
        }
        /// <summary>
        /// 提示记录不存在
        /// </summary>
        /// <returns></returns>
        protected new IActionResult NotFound()
        {
            return JResult.NotFound(T);
        }
        /// <summary>
        /// 提示没有权限
        /// </summary>
        /// <returns></returns>
        protected new IActionResult Unauthorized()
        {
            return JResult.Unauthorized(T);
        }
        #endregion
    }
}

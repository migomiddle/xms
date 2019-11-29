using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Xms.Infrastructure.Utility;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Filters;
using Xms.Web.Framework.Models;
using Xms.Web.Framework.Mvc;

namespace Xms.Web.Framework.Controller
{
    /// <summary>
    /// Web控制器基类
    /// </summary>
    [TypeFilter(typeof(OrganizationFilterAttribute), Order = -1)]
    [TypeFilter(typeof(ActionLoggingAttribute), Order = 100)]
    public class WebControllerBase : XmsControllerBase
    {
        protected WebControllerBase(IWebAppContext appContext) : base(appContext)
        {
        }

        #region 常用提示返回信息

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

        protected IActionResult JsonResult(object content, object extra = null)
        {
            return JResult.J("success", content, extra);
        }

        protected IActionResult JsonResult(bool isSuccess, object content, object extra = null)
        {
            return JResult.J(isSuccess, content, extra);
        }

        protected IActionResult JModelError(string title = "")
        {
            return JResult.Error((title.IsNotEmpty() ? title + ": " : "") + GetModelErrors());
        }

        #endregion 常用提示返回信息

        #region 常用返回结果

        /// <summary>
        /// 根据请求类型返回view或json
        /// </summary>
        /// <param name="model"></param>
        /// <param name="viewName"></param>
        /// <returns></returns>
        protected IActionResult DynamicResult(object model, string viewName = "")
        {
            if (IsRequestJson)
            {
                return JResult.J(model);
            }
            viewName = viewName.IfEmpty(WebContext.ActionName);
            return View(viewName, model);
        }

        /// <summary>
        /// 提示记录不存在
        /// </summary>
        /// <returns></returns>
        protected new IActionResult NotFound()
        {
            if (IsRequestJson)
            {
                return JResult.NotFound(T);
            }
            else
            {
                return PromptView(T["notfound_record"]);
            }
        }

        /// <summary>
        /// 提示没有权限
        /// </summary>
        /// <returns></returns>
        protected new IActionResult Unauthorized()
        {
            if (IsRequestJson)
            {
                return JResult.Unauthorized(T);
            }
            else
            {
                return PromptView(string.Format(T["security_unauthorized"], HttpContext.GetRouteString("org") + WebContext.LoginUrl));
            }
        }

        /// <summary>
        /// 提示有待完善
        /// </summary>
        /// <returns></returns>
        protected new IActionResult ToBePerfected()
        {
            return PromptView("莫慌！！！码农，正在马不停蹄的完善功能。。。");
        }

        /// <summary>
        /// 提示信息视图
        /// </summary>
        /// <param name="message">提示信息</param>
        /// <returns></returns>
        protected ViewResult PromptView(string message)
        {
            return View("Prompt", new PromptModel(WebContext.UrlReferrer, message));
        }

        /// <summary>
        /// 提示信息视图
        /// </summary>
        /// <param name="returnUrl">返回地址</param>
        /// <param name="message">提示信息</param>
        /// <returns></returns>
        protected ViewResult PromptView(string returnUrl, string message)
        {
            return View("Prompt", new PromptModel(returnUrl, message));
        }

        #endregion 常用返回结果
    }
}
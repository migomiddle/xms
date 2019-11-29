using Microsoft.AspNetCore.Mvc;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Web.Framework.Models;

namespace Xms.Web.Framework.Mvc
{
    /// <summary>
    /// 返回json格式
    /// </summary>
    public class JResult
    {
        public static IActionResult NotSpecifiedRecord(ILocalizedTextProvider _t, object extra = null)
        {
            return Error(_t["notspecified_record"], extra);
        }

        public static IActionResult NotFound(ILocalizedTextProvider _t, object extra = null)
        {
            return Error(_t["notfound_record"], extra);
        }

        public static IActionResult Unauthorized(ILocalizedTextProvider _t, object extra = null)
        {
            return Error(string.Format(_t["security_unauthorized"], "/account/signout"), extra);
        }

        public static IActionResult Error(object content, object extra = null)
        {
            return J(false, content, extra);
        }

        public static IActionResult Ok(object content, object extra = null)
        {
            return J(true, content, extra);
        }

        public static IActionResult Error(string content, object extra = null)
        {
            return J(false, content, extra);
        }

        public static IActionResult Ok(string content, object extra = null)
        {
            return J(true, content, extra);
        }

        /// <summary>
        /// json结果
        /// </summary>
        /// <param name="state">状态</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static IActionResult J(bool isSuccess, object content, object extra = null)
        {
            return new JsonResult(new JsonResultObject() { IsSuccess = isSuccess, Content = content is string ? content : content.SerializeToJson(), Extra = extra });
        }

        /// <summary>
        /// json结果
        /// </summary>
        /// <param name="statusName">状态</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static IActionResult J(string statusName, object content, object extra = null)
        {
            return new JsonResult(new JsonResultObject() { IsSuccess = true, StatusName = statusName, Content = content is string ? content : content.SerializeToJson(), Extra = extra });
        }

        /// <summary>
        /// json结果
        /// </summary>
        /// <param name="statusName">状态</param>
        /// <param name="content">内容</param>
        /// <returns></returns>
        public static IActionResult J(string statusName, string content, object extra = null)
        {
            return new JsonResult(new JsonResultObject() { IsSuccess = true, StatusName = statusName, Content = content, Extra = extra });
        }

        public static IActionResult J(object content)
        {
            return new JsonResult(new JsonResultObject() { IsSuccess = true, Content = content is string ? content : content.SerializeToJson() });
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Xms.Localization.Abstractions;

namespace Xms.Web.Framework.Mvc
{
    public static class ResultExtensions
    {
        #region 常用提示返回信息

        public static IActionResult CreateResult(this bool flag, ILocalizedTextProvider _t, object extra = null)
        {
            return JResult.J(flag, flag ? _t["created_success"] : _t["created_error"], extra);
        }

        public static IActionResult UpdateResult(this bool flag, ILocalizedTextProvider _t, object extra = null)
        {
            return JResult.J(flag, flag ? _t["updated_success"] : _t["updated_error"], extra);
        }

        public static IActionResult DeleteResult(this bool flag, ILocalizedTextProvider _t, object extra = null)
        {
            return JResult.J(flag, flag ? _t["deleted_success"] : _t["deleted_error"], extra);
        }

        public static IActionResult SaveResult(this bool flag, ILocalizedTextProvider _t, object extra = null)
        {
            return JResult.J(flag, flag ? _t["saved_success"] : _t["saved_error"], extra);
        }

        #endregion 常用提示返回信息
    }
}
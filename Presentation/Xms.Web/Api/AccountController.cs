using Microsoft.AspNetCore.Mvc;
using System;
using Xms.Core;
using Xms.Identity;
using Xms.Organization;
using Xms.Security.Verify;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Mvc;
using Xms.Web.Models;

namespace Xms.Web.Api
{
    /// <summary>
    /// 登录接口
    /// </summary>
    [Route("{org}/api/account")]
    [ApiController]
    public class AccountController : XmsControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly ISystemUserService _systemUserService;
        private readonly IVerifyProvider _verifyProvider;

        public AccountController(IWebAppContext appContext
            , IAuthenticationService authenticationService
            , ISystemUserService systemUserService
            , IVerifyProvider verifyProvider
           )
            : base(appContext)
        {
            _authenticationService = authenticationService;
            _systemUserService = systemUserService;
            _verifyProvider = verifyProvider;
        }

        [HttpPost]
        public IActionResult Post([FromForm]SignInModel model)
        {
            var flag = false;
            var msg = string.Empty;
            if (ModelState.IsValid)
            {
                if (WebContext.PlatformSettings.VerifyCodeEnabled && !_verifyProvider.IsValid(model.VerifyCode))
                {
                    flag = false;
                    msg = "验证码不正确";
                    ModelState.AddModelError("verifycode", msg);
                }
                else
                {
                    var orgInfo = WebContext.Org;
                    var u = _systemUserService.GetUserByLoginName(model.LoginName);
                    if (u == null)
                    {
                        flag = false;
                        msg = "帐号不存在";
                        ModelState.AddModelError("loginname", msg);
                    }
                    else if (u.StateCode == RecordState.Disabled)
                    {
                        flag = false;
                        msg = "帐号已禁用";
                        ModelState.AddModelError("loginname", msg);
                    }
                    else
                    {
                        if (_systemUserService.IsValidePassword(model.Password, u.Salt, u.Password))
                        {
                            //登录状态记录
                            _authenticationService.SignIn(u, true);
                            //获取用户个性化信息

                            //更新最后登录时间
                            _systemUserService.Update(n => n.Set(f => f.LastLoginTime, DateTime.Now)//.Set(f => f.IsLogin, true)
                            .Where(f => f.SystemUserId == u.SystemUserId));
                            msg = "登录成功";
                            flag = true;
                        }
                        else
                        {
                            flag = false;
                            msg = "密码有误";
                            ModelState.AddModelError("password", msg);
                        }
                    }
                }
                if (flag)
                {
                    return JResult.Ok(msg);
                }
            }
            return JResult.Error(GetModelErrors());
        }
    }
}
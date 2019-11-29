using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.ComponentModel;
using Xms.Configuration;
using Xms.Configuration.Domain;
using Xms.Infrastructure.Utility;
using Xms.Logging.Abstractions;
using Xms.Notify.Email;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Framework.Mvc;
using Xms.Web.Models;

namespace Xms.Web.Api
{
    /// <summary>
    /// 参数配置接口
    /// </summary>
    [Route("{org}/api/settings")]
    public class SettingsController : ApiControllerBase
    {
        private readonly ISettingService _settingService;
        private readonly ISettingFinder _settingFinder;

        public SettingsController(IWebAppContext appContext
            , ISettingService settingService
            , ISettingFinder settingFinder)
            : base(appContext)
        {
            _settingService = settingService;
            _settingFinder = settingFinder;
        }

        #region 系统参数

        [HttpPost("platform")]
        [Description("编辑系统参数")]
        public IActionResult SavePlatformSettings([FromBody]PlatformSettingModel model)
        {
            if (ModelState.IsValid)
            {
                PlatformSetting setting = new PlatformSetting
                {
                    AppName = model.AppName
                    ,
                    VersionNumber = model.AppVersion
                    ,
                    DataLogEnabled = model.DataLogEnabled
                    ,
                    LogLevel = (LogLevel)model.LogLevel
                    ,
                    LogEnabled = model.LogEnabled
                    ,
                    ShowMenuInUserPrivileges = model.ShowMenuInUserPrivileges
                    ,
                    VerifyCodeEnabled = model.VerifyCodeEnabled
                    ,
                    MaxFetchRecords = model.MaxFetchRecords
                    ,
                    CacheEnabled = model.CacheEnabled
                };
                return _settingService.Save(setting).SaveResult(T);
            }
            return SaveFailure(GetModelErrors());
        }

        #endregion 系统参数

        #region 短信

        [HttpPost("sms")]
        [Description("编辑短信参数")]
        public IActionResult SaveSmsSettings([FromBody]SmsSettingModel model)
        {
            if (ModelState.IsValid)
            {
                SmsSetting entity = new SmsSetting();

                model.CopyTo(entity);
                int i = 0;
                List<SmsBody> bodies = new List<SmsBody>();
                foreach (var item in model.Name)
                {
                    if (item.IsNotEmpty() && model.Content[i].IsNotEmpty())
                    {
                        bodies.Add(new SmsBody() { Name = item, Content = model.Content[i] });
                    }
                    i++;
                }
                entity.Bodies = bodies;
                return _settingService.Save(entity).SaveResult(T);
            }
            return SaveFailure(GetModelErrors());
        }

        #endregion 短信

        #region email

        [HttpPost("email")]
        [Description("编辑电子邮件参数")]
        public IActionResult SaveEmailSettings([FromBody]EmailSettingModel model)
        {
            if (ModelState.IsValid)
            {
                EmailSetting entity = new EmailSetting();

                model.CopyTo(entity);
                int i = 0;
                List<EmailBody> bodies = new List<EmailBody>();
                foreach (var item in model.Name)
                {
                    if (item.IsNotEmpty() && model.Content[i].IsNotEmpty())
                    {
                        bodies.Add(new EmailBody() { Name = item, Content = model.Content[i] });
                    }
                    i++;
                }
                entity.Bodies = bodies;
                return _settingService.Save(entity).SaveResult(T);
            }
            return SaveFailure(GetModelErrors());
        }

        [HttpPost("sendemails")]
        [Description("发送邮件")]
        public IActionResult SendEmails([FromBody]SendEmailsModel model)
        {
            if (ModelState.IsValid)
            {
                var emails = model.EmailAddresses.SplitSafe(",");
                if (emails.IsEmpty())
                {
                    return JError(T["noneemailsettings"]);
                }
                EmailSetting emailSettings = _settingFinder.Get<EmailSetting>();
                if (emailSettings.Host.IsEmpty())
                {
                    return JError(T["noneemailsettings"]);
                }
                var _emailMessageHandler = new EmailMessageHandler();
                foreach (var em in emails)
                {
                    _emailMessageHandler.Send(new EmailNotifyBody()
                    {
                        Subject = model.Subject
                        ,
                        Content = model.Content
                        ,
                        Host = emailSettings.Host
                        ,
                        Port = int.Parse(emailSettings.Port)
                        ,
                        UserName = emailSettings.UserName
                        ,
                        Password = emailSettings.Password
                        ,
                        From = emailSettings.From
                        ,
                        FromName = emailSettings.FromName
                        ,
                        To = em
                    });
                }
                return JOk(T["operation_success"]);
            }
            return JError(T["operation_error"] + ": " + GetModelErrors());
        }

        #endregion email

        #region 上传

        [HttpPost("upload")]
        [Description("保存上传文件参数")]
        public IActionResult SaveUploadSettings([FromBody]UploadSettingModel model)
        {
            if (ModelState.IsValid)
            {
                UploadSetting entity = new UploadSetting();

                model.CopyTo(entity);
                return _settingService.Save(entity).SaveResult(T);
            }
            return SaveFailure(GetModelErrors());
        }

        #endregion 上传

        #region 缓存

        [HttpPost("cache")]
        [Description("保存缓存参数")]
        public IActionResult SaveCacheSettings([FromBody]CacheSettingModel model)
        {
            if (ModelState.IsValid)
            {
                CacheSetting entity = new CacheSetting();

                model.CopyTo(entity);
                return _settingService.Save(entity).SaveResult(T);
            }
            return SaveFailure(GetModelErrors());
        }

        #endregion 缓存
    }
}
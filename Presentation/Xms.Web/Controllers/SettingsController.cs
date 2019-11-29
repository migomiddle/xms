using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using Xms.Configuration;
using Xms.Configuration.Domain;
using Xms.Infrastructure.Utility;
using Xms.Web.Framework.Context;
using Xms.Web.Framework.Controller;
using Xms.Web.Models;

namespace Xms.Web.Controllers
{
    /// <summary>
    /// 配置管理控制器
    /// </summary>
    public class SettingsController : AuthorizedControllerBase
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

        [HttpGet]
        [Description("编辑系统参数")]
        public IActionResult EditPlatformSetting()
        {
            var settings = _settingFinder.Get<PlatformSetting>();
            PlatformSettingModel model = new PlatformSettingModel
            {
                AppName = settings.AppName,
                AppVersion = settings.VersionNumber,
                DataLogEnabled = settings.DataLogEnabled,
                LogEnabled = settings.LogEnabled,
                LogLevel = (int)settings.LogLevel,
                ShowMenuInUserPrivileges = settings.ShowMenuInUserPrivileges,
                MaxFetchRecords = settings.MaxFetchRecords,
                CacheEnabled = settings.CacheEnabled
            };

            return View(model);
        }

        #endregion 系统参数

        #region 短信

        [HttpGet]
        [Description("编辑短信参数")]
        public IActionResult EditSmsSetting()
        {
            SmsSettingModel model = new SmsSettingModel();
            var settings = _settingFinder.Get<SmsSetting>();

            if (settings != null)
            {
                settings.CopyTo(model);
            }

            return View(model);
        }

        #endregion 短信

        #region email

        [HttpGet]
        [Description("编辑电子邮件参数")]
        public IActionResult EditEmailSetting()
        {
            EmailSettingModel model = new EmailSettingModel();
            EmailSetting entity = _settingFinder.Get<EmailSetting>();

            if (entity != null)
            {
                entity.CopyTo(model);
            }

            return View(model);
        }

        [HttpGet]
        [Description("发送邮件")]
        public IActionResult SendEmails()
        {
            SendEmailsModel model = new SendEmailsModel();
            return View(model);
        }

        #endregion email

        #region 上传

        [HttpGet]
        [Description("编辑上传文件参数")]
        public IActionResult EditUploadSetting()
        {
            UploadSettingModel model = new UploadSettingModel();
            var settings = _settingFinder.Get<UploadSetting>();

            if (settings != null)
            {
                settings.CopyTo(model);
            }

            return View(model);
        }

        #endregion 上传

        #region 缓存

        [HttpGet]
        [Description("编辑上传文件参数")]
        public IActionResult EditCacheSetting()
        {
            CacheSettingModel model = new CacheSettingModel();
            var settings = _settingFinder.Get<CacheSetting>();

            if (settings != null)
            {
                settings.CopyTo(model);
            }

            return View(model);
        }

        #endregion 缓存
    }
}
using Microsoft.AspNetCore.Http;
using System;
using Xms.Configuration.Domain;
using Xms.Localization.Abstractions;

namespace Xms.Context
{
    public interface IAppContext
    {
        HttpContext HttpContext { get; }

        /// <summary>
        /// //当前请求是否为ajax请求
        /// </summary>
        bool IsAjaxRequest { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        string IP { get; set; }

        /// <summary>
        /// 当前访问的URL
        /// </summary>
        string Url { get; set; }

        /// <summary>
        /// 上一次访问的url
        /// </summary>
        string UrlReferrer { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        string Area { get; set; }

        /// <summary>
        /// 控制器名称
        /// </summary>
        string ControllerName { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        string ActionName { get; set; }

        Guid OrganizationId { get; }

        /// <summary>
        /// 当前组织唯一名称
        /// </summary>
        string OrganizationUniqueName { get; set; }

        /// <summary>
        /// 当前组织名称
        /// </summary>
        string OrganizationName { get; set; }

        /// <summary>
        /// 页面布局方式
        /// </summary>
        int LayoutType { get; set; }

        /// <summary>
        /// 页面标题
        /// </summary>
        string PageTitle { get; set; }

        /// <summary>
        /// 是否已登录
        /// </summary>
        bool IsSignIn { get; }

        /// <summary>
        /// 登录地址
        /// </summary>
        string LoginUrl { get; }

        /// <summary>
        /// 登出地址
        /// </summary>
        string LogoutUrl { get; }

        /// <summary>
        /// 初始化地址
        /// </summary>
        string InitializationUrl { get; }
        bool IsInitialization { get; set; }
        LanguageCode BaseLanguage { get; }

        T GetFeature<T>();

        /// <summary>
        /// 主题
        /// </summary>
        string Theme { get; set; }

        /// <summary>
        /// 用户个性化
        /// </summary>

        string UserPersonalizations { get; }

        /// <summary>
        /// 平台参数
        /// </summary>
        PlatformSetting PlatformSettings { get; set; }
    }
}
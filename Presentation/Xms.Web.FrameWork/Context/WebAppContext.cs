using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xms.Configuration;
using Xms.Configuration.Domain;
using Xms.Core.Org;
using Xms.Identity;
using Xms.Infrastructure;
using Xms.Infrastructure.Utility;
using Xms.Localization.Abstractions;
using Xms.Organization;
using Xms.Security.Principal;
using Xms.SiteMap;
using Xms.SiteMap.Domain;
using Xms.UserPersonalization;

namespace Xms.Web.Framework.Context
{
    /// <summary>
    /// web上下文
    /// </summary>
    public class WebAppContext : IWebAppContext
    {
        private readonly IAuthenticationService _authenticationService;
        private readonly IOrganizationService _organizationService;
        private readonly ISettingFinder _settingFinder;
        private readonly ISystemUserRolesService _systemUserRolesService;
        private readonly IUserPersonalizationService _userPersonalizationService;

        public WebAppContext(IHttpContextAccessor httpContext
            , IAuthenticationService authenticationService
            , IOrgDataServer orgDataServer
            , IOrganizationService organizationService
            , ISettingFinder settingFinder
            , ISystemUserRolesService systemUserRolesService
            , ILocalizedTextProvider localizedTextProvider
            , IUserPersonalizationService userPersonalizationService
            )
        {
            HttpContext = httpContext.HttpContext;
            _authenticationService = authenticationService;
            OrgDataServer = orgDataServer;
            _organizationService = organizationService;
            _settingFinder = settingFinder;
            _systemUserRolesService = systemUserRolesService;
            T = localizedTextProvider;
            _userPersonalizationService = userPersonalizationService;
            SetFeatures();
        }

        /// <summary>
        /// 组织数据服务器信息
        /// </summary>
        public IOrgDataServer OrgDataServer
        {
            get; set;
        }

        public HttpContext HttpContext { get; }

        /// <summary>
        /// //当前请求是否为ajax请求
        /// </summary>
        public bool IsAjaxRequest { get; set; }

        /// <summary>
        /// 客户端IP
        /// </summary>
        public string IP { get; set; }

        /// <summary>
        /// 当前访问的URL
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 上一次访问的url
        /// </summary>
        public string UrlReferrer { get; set; }

        /// <summary>
        /// 区域
        /// </summary>
        public string Area { get; set; }

        /// <summary>
        /// 控制器名称
        /// </summary>
        public string ControllerName { get; set; }

        /// <summary>
        /// 方法名称
        /// </summary>
        public string ActionName { get; set; }

        public Guid OrganizationId
        {
            get
            {
                return Org != null ? Org.OrganizationId : Guid.Empty;
            }
        }

        private string _organizationUniqueName;

        /// <summary>
        /// 当前组织唯一名称
        /// </summary>
        public string OrganizationUniqueName
        {
            get
            {
                if (_organizationUniqueName.IsEmpty())
                {
                    if ((_organizationUniqueName = HttpContext.GetRouteValue("org")?.ToString()).IsNotEmpty())
                    {
                        //var routeValues = OrgRouteMatcher.Match(_httpContext);
                        //_organizationUniqueName = routeValues["org"]?.ToString() ?? "";
                    }
                    else if (_orgInfo != null)
                    {
                        _organizationUniqueName = _orgInfo.UniqueName;
                    }
                }
                return _organizationUniqueName;
            }
            set
            {
                _organizationUniqueName = value;
            }
        }

        private string _organizationName;

        /// <summary>
        /// 当前组织名称
        /// </summary>
        public string OrganizationName
        {
            get
            {
                if (_orgInfo != null)
                {
                    return _orgInfo.Name;
                }
                return _organizationName;
            }
            set
            {
                _organizationName = value;
            }
        }

        private Organization.Domain.Organization _orgInfo;

        /// <summary>
        /// 组织信息
        /// </summary>
        public Organization.Domain.Organization Org
        {
            get
            {
                if (_orgInfo == null && OrganizationUniqueName.HasValue())
                {
                    if (OrgDataServer != null && !OrgDataServer.OrganizationBaseId.Equals(Guid.Empty))
                    {
                        _orgInfo = _organizationService.FindById(OrgDataServer.OrganizationBaseId);
                    }
                    else
                    {
                        throw new XmsNotFoundException("组织不存在");
                    }
                }
                return _orgInfo;
            }
            set
            {
                _orgInfo = value;
            }
        }

        private ICurrentUser _currentUser;

        /// <summary>
        /// 当前用户信息
        /// </summary>
        public ICurrentUser CurrentUser
        {
            get
            {
                if ((_currentUser == null || !_currentUser.HasValue()) && OrganizationUniqueName.HasValue())
                {
                    _currentUser = _authenticationService.GetAuthenticatedUser();
                    if (_currentUser != null)
                    {
                        _currentUser.OrgInfo = Org;
                        _currentUser.IsSuperAdmin = _systemUserRolesService.IsAdministrator(_currentUser.SystemUserId);
                        _currentUser.Roles = _systemUserRolesService.FindByUserId(_currentUser.SystemUserId);
                    }
                }
                return _currentUser;
            }
            set
            {
                _currentUser = value;
            }
        }

        private PlatformSetting _platformSettings;

        /// <summary>
        /// 平台参数
        /// </summary>
        public PlatformSetting PlatformSettings
        {
            get
            {
                if (_platformSettings == null)
                {
                    _platformSettings = _settingFinder.Get<PlatformSetting>();
                }
                return _platformSettings;
            }
            set
            {
                _platformSettings = value;
            }
        }

        /// <summary>
        /// 语言服务
        /// </summary>
        public ILocalizedTextProvider T { get; set; }

        /// <summary>
        /// 页面布局方式
        /// </summary>
        public int LayoutType
        {
            get
            {
                return CurrentUser?.UserSettings?.LayoutType ?? 0;
            }
            set { }
        }

        private string _pageTitle;

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle
        {
            get
            {
                if (!_pageTitle.HasValue())
                {
                    if (PrivilegeTree.NotEmpty())
                    {
                        var names = PrivilegeTree.Select(s => s.DisplayName).ToList();
                        if (PlatformSettings != null)
                        {
                            names.Add(PlatformSettings.AppName);
                        }
                        _pageTitle = string.Join(" - ", names);
                    }
                }
                return _pageTitle;
            }
            set
            {
                _pageTitle = value;
            }
        }

        private List<Privilege> _privilegeTree;

        public List<Privilege> PrivilegeTree
        {
            get
            {
                if (_privilegeTree == null)
                {
                    var url = HttpContext.GetThisPageUrl(false);
                    if (url.IsNotEmpty())
                    {
                        url = url.Replace("/" + OrganizationUniqueName, "");
                        _privilegeTree = ((IPrivilegeTreeBuilder)HttpContext.RequestServices.GetService(typeof(IPrivilegeTreeBuilder))).GetTreePath(url);
                    }
                }
                return _privilegeTree;
            }
        }

        /// <summary>
        /// 是否已登录
        /// </summary>
        public bool IsSignIn
        {
            get
            {
                return this.CurrentUser != null && this.CurrentUser.HasValue();
            }
        }

        /// <summary>
        /// 登录地址
        /// </summary>
        public string LoginUrl
        {
            get
            {
                return XmsAuthenticationDefaults.LoginPath;
            }
        }

        /// <summary>
        /// 登出地址
        /// </summary>
        public string LogoutUrl
        {
            get
            {
                return XmsAuthenticationDefaults.LogoutPath;
            }
        }

        /// <summary>
        /// 初始化地址
        /// </summary>
        public string InitializationUrl
        {
            get
            {
                return XmsAuthenticationDefaults.InitializationPath;
            }
        }
        private string _isInitialization;
        public bool IsInitialization
        {
            get
            {
                if (_isInitialization == null)
                {
                    var builder = new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json");
                    var config = builder.Build();
                    _isInitialization = config["Initialization:IsInitialization"] == "true" ? "true" : null;
                }
                return _isInitialization == "true";
            }
            set
            {
                if (value)
                {
                    _isInitialization = "true";
                }
                else
                {
                    _isInitialization = "false";
                }
            }
        }
        public LanguageCode BaseLanguage
        {
            get
            {
                if (Org != null)
                {
                    return Org.LanguageId;
                }
                return LanguageCode.CHS;
            }
        }

        public T GetFeature<T>()
        {
            return HttpContext.Features.Get<T>();
        }

        private void SetFeatures()
        {
            HttpContext.Features.Set(Org);
            HttpContext.Features.Set(CurrentUser);
            HttpContext.Features.Set(T);
        }

        /// <summary>
        /// 主题
        /// </summary>
        public string Theme { get; set; }

        /// <summary>
        /// 用户个性化
        /// </summary>
        private string _userPersonalizations;

        public string UserPersonalizations
        {
            get
            {
                if (_userPersonalizations.IsEmpty() && this.CurrentUser != null && this.CurrentUser.HasValue())
                {
                    _userPersonalizations = _userPersonalizationService.Get(this.CurrentUser.SystemUserId).SerializeToJson();
                }
                return _userPersonalizations;
            }
        }
    }
}
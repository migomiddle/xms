using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;
using System.Security.Claims;
using Xms.Infrastructure.Utility;
using Xms.Organization;
using Xms.Organization.Domain;
using Xms.Session;

namespace Xms.Identity
{
    /// <summary>
    /// 身份认证服务
    /// </summary>
    public class ClaimAuthenticationService : IAuthenticationService
    {
        private readonly HttpContext _httpContext;
        private readonly ISystemUserService _userService;
        private readonly long _expiration = 30;
        private readonly ISessionService _sessionService;
        private ICurrentUser _cachedUser;

        public ClaimAuthenticationService(IHttpContextAccessor httpContext
            , ISessionService sessionService
            , ISystemUserService userService
            , ICurrentUser currentUser
            )
        {
            _httpContext = httpContext.HttpContext;
            _sessionService = sessionService;
            _userService = userService;
            _cachedUser = currentUser;
        }

        public virtual async void SignIn(SystemUser user, bool persistent = true)
        {
            var claims = new[] {
                new Claim("Name", user.LoginName, ClaimValueTypes.String, XmsAuthenticationDefaults.ClaimsIssuer)
                ,new Claim("Org", user.OrganizationId.ToString(), ClaimValueTypes.String, XmsAuthenticationDefaults.ClaimsIssuer)
                ,new Claim("OrgUniqueName", user.UniqueName.ToString(), ClaimValueTypes.String, XmsAuthenticationDefaults.ClaimsIssuer)
            };
            var userIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            ClaimsPrincipal userPrincipal = new ClaimsPrincipal(userIdentity);
            var authenticationProperties = new AuthenticationProperties
            {
                IsPersistent = persistent
                ,
                IssuedUtc = DateTime.UtcNow
                ,
                ExpiresUtc = DateTimeOffset.Now.AddMinutes(_expiration)
            };
            await _httpContext.SignInAsync(
            XmsAuthenticationDefaults.AuthenticationScheme,
            userPrincipal, authenticationProperties);

            _cachedUser.LoginName = user.LoginName;
            _cachedUser.SystemUserId = user.SystemUserId;
            _cachedUser.UserName = user.Name;
            _cachedUser.OrganizationId = user.OrganizationId;
            _cachedUser.BusinessUnitId = user.BusinessUnitId;
            _cachedUser.SessionId = _sessionService.GetId();

            _sessionService.Set(CurrentUser.SESSION_KEY, _cachedUser);
        }

        public virtual async void SignOut()
        {
            _cachedUser = null;
            _httpContext.Session.Clear();
            await _httpContext.SignOutAsync(XmsAuthenticationDefaults.AuthenticationScheme);
        }

        public virtual ICurrentUser GetAuthenticatedUser()
        {
            if (_cachedUser != null && _cachedUser.HasValue())
            {
                return _cachedUser;
            }

            if (_httpContext?.User == null || _httpContext?.User?.Identity == null || !_httpContext.User.Identity.IsAuthenticated)
            {
                return null;
            }
            var claims = _httpContext.User.Claims.ToList();
            if (claims.Count() < 3)
            {
                return null;
            }
            var authenticateResult = _httpContext.AuthenticateAsync(XmsAuthenticationDefaults.AuthenticationScheme).Result;
            if (!authenticateResult.Succeeded)
            {
                return null;
            }
            var nameClaim = authenticateResult.Principal.FindFirst(claim => claim.Type.IsCaseInsensitiveEqual("Name"));
            //&& claim.Issuer.Equals(XmsAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase)
            if (nameClaim == null)
            {
                return null;
            }
            var groupClaim = authenticateResult.Principal.Claims.FirstOrDefault(claim => claim.Type == "Org");
            //&& claim.Issuer.Equals(XmsAuthenticationDefaults.ClaimsIssuer, StringComparison.InvariantCultureIgnoreCase)
            if (groupClaim == null)
            {
                return null;
            }

            return GetAuthenticatedUserFromSession(nameClaim, groupClaim) ?? GetAuthenticatedUserFromTicket(nameClaim, groupClaim);
        }

        private ICurrentUser GetAuthenticatedUserFromSession(Claim nameClaim, Claim groupClaim)
        {
            var loginName = nameClaim.Value;
            var organizationId = groupClaim.Value;

            var user = _sessionService.GetValue<CurrentUser>(CurrentUser.SESSION_KEY);

            if (user != null && user.OrganizationId.ToString().IsCaseInsensitiveEqual(organizationId) && user.LoginName.IsCaseInsensitiveEqual(loginName))
            {
                return user;
            }

            return null;
        }

        private ICurrentUser GetAuthenticatedUserFromTicket(Claim nameClaim, Claim groupClaim)
        {
            var loginName = nameClaim.Value;
            //var organizationId = groupClaim.Value;
            var user = _userService.GetUserByLoginName(loginName);
            if (user == null)
            {
                return null;
            }
            //密码已更改
            //if (!user.Password.IsCaseInsensitiveEqual(password))
            //{
            //    return null;
            //}
            //已删除或禁用
            if (user.IsDeleted || user.StateCode == 0)
            {
                return null;
            }
            _cachedUser.LoginName = user.LoginName;
            _cachedUser.SystemUserId = user.SystemUserId;
            _cachedUser.UserName = user.Name;
            _cachedUser.OrganizationId = user.OrganizationId;
            _cachedUser.BusinessUnitId = user.BusinessUnitId;
            _cachedUser.SessionId = _sessionService.GetId();
            _sessionService.Set(CurrentUser.SESSION_KEY, _cachedUser);
            return _cachedUser;
        }
    }
}
using Microsoft.AspNetCore.Http;

namespace Xms.Identity
{
    public static class XmsAuthenticationDefaults
    {
        public static string AuthenticationScheme => "Authentication";
        public static string ExternalAuthenticationScheme => "ExternalAuthentication";
        public static string ClaimsIssuer => "xms";

        public static PathString LoginPath => new PathString("/account/signin");

        public static PathString LogoutPath => new PathString("/account/signout");
        public static PathString AccessDeniedPath => new PathString("/error");

        public static PathString InitializationPath => new PathString("/initialization/initialization");
    }

    public static class XmsCookieDefaults
    {
        public static string Prefix => ".Xms";

        public static string UserCookie => ".User";

        public static string AntiforgeryCookie => ".Antiforgery";

        public static string SessionCookie => ".Session";
        public static string AuthenticationCookie => ".Authentication";

        public static string ExternalAuthenticationCookie => ".ExternalAuthentication";
    }
}
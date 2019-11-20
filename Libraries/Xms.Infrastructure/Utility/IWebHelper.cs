namespace Xms.Infrastructure.Utility
{
    public interface IWebHelper
    {
        string MakeAllUrlsAbsolute(string html, string protocol, string host);

        string MapPath(string path, bool autoCreate = false);

        void RestartAppDomain(bool makeRedirect = false, string redirectUrl = "", bool aggressive = false);
    }
}
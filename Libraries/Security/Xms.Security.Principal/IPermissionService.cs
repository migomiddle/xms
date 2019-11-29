namespace Xms.Security.Principal
{
    public interface IPermissionService
    {
        bool HasPermission(string areaName, string className, string methodName, bool ignoreNull = true);

        bool HasPermission(string url, bool ignoreNull = true);
    }
}
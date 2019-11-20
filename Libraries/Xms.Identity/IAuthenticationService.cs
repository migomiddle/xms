using Xms.Organization.Domain;

namespace Xms.Identity
{
    public interface IAuthenticationService
    {
        ICurrentUser GetAuthenticatedUser();

        void SignIn(SystemUser user, bool persistent = true);

        void SignOut();
    }
}
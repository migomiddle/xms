using IdentityServer4.Validation;
using System.Security.Claims;
using System.Threading.Tasks;
using Xms.Infrastructure.Utility;
using Xms.Organization;

namespace Xms.Api.Identity.Services
{
    public class CustomResourceOwnerPasswordValidator : IResourceOwnerPasswordValidator
    {
        private readonly ISystemUserService _systemUserService;
        //private readonly string _orgUniqueName;

        public CustomResourceOwnerPasswordValidator(ISystemUserService systemUserService)//, IHttpContextAccessor httpContext)
        {
            _systemUserService = systemUserService;
            //_orgUniqueName = httpContext.HttpContext.GetRouteOrQueryString("org")?.ToString();
        }

        public Task ValidateAsync(ResourceOwnerPasswordValidationContext context)
        {
            var user = _systemUserService.GetUserByLoginNameAndPassword(context.UserName, SecurityHelper.MD5(context.Password));
            if (user != null)
            {
                context.Result = new GrantValidationResult(
                        subject: user.SystemUserId.ToString(),
                        authenticationMethod: "custom",
                        claims: new Claim[] {
                            new Claim("Name", context.UserName),
                            new Claim("Org", user.OrganizationId.ToString())
                        });
            }
            return Task.FromResult(0);
        }
    }
}

using IdentityServer4.Extensions;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Xms.Organization;

namespace Xms.Api.Identity.Services
{
    public class CustomProfileService : IProfileService
    {
        private readonly ILogger _logger;
        //private readonly ISystemUserService _systemUserService;

        public CustomProfileService(//ISystemUserService systemUserService, 
            ILogger<CustomProfileService> logger)
        {
            //_systemUserService = systemUserService;
            _logger = logger;
        }


        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            _logger.LogDebug("Get profile called for subject {subject} from client {client} with claim types {claimTypes} via {caller}",
                context.Subject.GetSubjectId(),
                context.Client.ClientName ?? context.Client.ClientId,
                context.RequestedClaimTypes,
                context.Caller);

            //var user = _systemUserService.FindById(new Guid(context.Subject.GetSubjectId()));

            //var claims = new List<Claim>
            //{
            //    new Claim("Name", user.LoginName),
            //    new Claim("Org", user.OrganizationId.ToString())
            //};
            var claims = context.Subject.Claims.ToList();
            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            context.IsActive = true;
        }
    }
}

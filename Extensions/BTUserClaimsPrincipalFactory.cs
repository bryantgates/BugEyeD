using System.Security.Claims;
using BugEyeD.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace BugEyeD.Extensions
{
    public class BTUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<BTUser, IdentityRole>
    {
        public BTUserClaimsPrincipalFactory(UserManager<BTUser> userManager,
                                            RoleManager<IdentityRole> roleManager,
                                            IOptions<IdentityOptions> options) 
            : base(userManager, roleManager, options)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(BTUser user)
        {
            ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

            Claim companyClaim = new Claim("CompanyID", user.CompanyId.ToString());
            
            identity.AddClaim(companyClaim);

            return identity;
        }
    }
}

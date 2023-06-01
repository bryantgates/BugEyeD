using System.Security.Claims;
using System.Security.Principal;

namespace BugEyeD.Extensions
{
    public static class IdentityExtentions
    {
        public static int GetCompanyId(this IIdentity identity)
        {
            Claim claim = ((ClaimsIdentity)identity).FindFirst("CompanyId")!;
            return int.Parse(claim.Value);
        }
    }
}

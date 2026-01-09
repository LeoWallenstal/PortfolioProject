using DataLayer.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;

namespace DataLayer.Data
{
    public class CustomClaimsFactory : UserClaimsPrincipalFactory<User>
    {
        public CustomClaimsFactory(
            UserManager<User> userManager,
            IOptions<IdentityOptions> optionsAccessor)
            : base(userManager, optionsAccessor)
        {
        }

        protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
        {
            var identity = await base.GenerateClaimsAsync(user);
            identity.AddClaim(new Claim("IsActive", user.IsActive.ToString().ToLower()));

            return identity;
        }
    }
}

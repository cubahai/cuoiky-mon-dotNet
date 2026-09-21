using BanHangDienTu.Models.Constants;
using BanHangDienTu.Models.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BanHangDienTu.Services.Identity;

public sealed class ApplicationUserClaimsPrincipalFactory
    : UserClaimsPrincipalFactory<ApplicationUser, IdentityRole>
{
    public ApplicationUserClaimsPrincipalFactory(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IOptions<IdentityOptions> optionsAccessor)
        : base(
            userManager,
            roleManager,
            optionsAccessor)
    {
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(
        ApplicationUser user)
    {
        var identity =
            await base.GenerateClaimsAsync(user);

        identity.AddClaim(
            new Claim(
                AppClaimTypes.FullName,
                user.FullName));

        return identity;
    }
}
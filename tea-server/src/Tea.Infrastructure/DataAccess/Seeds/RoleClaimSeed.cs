using Microsoft.AspNetCore.Identity;
using Tea.Domain.Constants;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.DataAccess.Seeds
{
    public class RoleClaimSeed
    {
        public static async Task SeedAsync(TeaContext context, RoleManager<AppRole> roleManager)
        {
            if (context.RoleClaims.Any()) return;

            var adminRole = await roleManager.FindByNameAsync("Admin");
            var adminClaims = AppPermission.adminClaims.Select(claim => new IdentityRoleClaim<string>
            {
                RoleId = adminRole.Id,
                ClaimType = claim.ClaimType,
                ClaimValue = claim.ClaimValue,
            }).ToList();

            await context.RoleClaims.AddRangeAsync(adminClaims);
            await context.SaveChangesAsync();
        }
    }
}

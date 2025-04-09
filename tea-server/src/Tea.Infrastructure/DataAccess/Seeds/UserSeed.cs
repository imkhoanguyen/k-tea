using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Tea.Domain.Entities;

namespace Tea.Infrastructure.DataAccess.Seeds
{
    public class UserSeed
    {
        public static async Task SeedAsync(UserManager<AppUser> userManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            var users = new List<AppUser>
            {
                new AppUser
                {
                    FullName = "Nguyễn Anh Khoa",
                    UserName = "Admin",
                    Email = "khoanguyen.coder@gmail.com",
                    PhoneNumber = "0985576590",
                    Address = "Quận 11 TP Hồ Chí Minh"
                },
                new AppUser
                {
                    FullName = "Nguyễn Văn B",
                    UserName = "Customer",
                    Email = "itk21sgu@gmail.com",
                    PhoneNumber = "0147258369",
                    Address = "Quận 11 TP Hồ Chí Minh"
                }
            };

            foreach (var user in users)
            {
                await userManager.CreateAsync(user, "Admin_123");
            }

            var adminUser = await userManager.FindByNameAsync("Admin");

            await userManager.AddToRoleAsync(adminUser, "Admin");

            var customerUser = await userManager.FindByNameAsync("Customer");

            await userManager.AddToRoleAsync(customerUser, "Customer");
        }
    }
}

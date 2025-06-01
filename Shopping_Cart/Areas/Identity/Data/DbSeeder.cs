using Microsoft.AspNetCore.Identity;
using Shopping_Cart.Constants;
namespace Shopping_Cart.Areas.Identity.Data
{
    public class DbSeeder
    {
        public static async Task SeedDefaultData(IServiceProvider service)
        {
            var userMgr = service.GetService<UserManager<IdentityUser>>();
            var roleMgr = service.GetService<RoleManager<IdentityRole>>();

            if (roleMgr == null || userMgr == null)
                return;

            // Create Roles if they do not exist
            if (!await roleMgr.RoleExistsAsync(Roles.Admin.ToString()))
                await roleMgr.CreateAsync(new IdentityRole(Roles.Admin.ToString()));

            if (!await roleMgr.RoleExistsAsync(Roles.User.ToString()))
                await roleMgr.CreateAsync(new IdentityRole(Roles.User.ToString()));

            // Check if Admin user already exists
            var admin = new IdentityUser
            {
                UserName = "admin@gmail.com",
                Email = "admin@gmail.com",
                EmailConfirmed = true
            };

            var userInDb = await userMgr.FindByEmailAsync(admin.Email);

            if (userInDb == null)
            {
                var result = await userMgr.CreateAsync(admin, "Admin@123");

                if (result.Succeeded)
                {
                    await userMgr.AddToRoleAsync(admin, Roles.Admin.ToString());
                }
            }
        }
    }
}

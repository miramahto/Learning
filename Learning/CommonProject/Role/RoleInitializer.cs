using AuthApi.Models;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

public class RoleInitializer
{
    public static async Task InitializeAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
    {
        string adminEmail = "admin@example.com";
        string password = "Admin@12345";
        if (await roleManager.FindByNameAsync("admin") == null)
        {
            await roleManager.CreateAsync(new IdentityRole("admin"));
        }
        if (await roleManager.FindByNameAsync("user") == null)
        {
            await roleManager.CreateAsync(new IdentityRole("user"));
        }
        if (await userManager.FindByNameAsync(adminEmail) == null)
        {
            ApplicationUser admin = new ApplicationUser { Email = adminEmail, UserName = adminEmail };
            IdentityResult result = await userManager.CreateAsync(admin, password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(admin, "admin");
            }
        }
    }
}
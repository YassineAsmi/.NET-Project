using Microsoft.AspNetCore.Identity;

namespace WebProject.Data
{
    public class RoleInitializer
    {
        public static async Task EnsureRolesCreated(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            string[] roles = new string[] { WebSiteRole.Admin, WebSiteRole.Manager, WebSiteRole.User };

            foreach (string role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }
    }
}

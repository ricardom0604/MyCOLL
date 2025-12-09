using Microsoft.AspNetCore.Identity;
using MyCOLLDB.Data;
using MyCOLLDB.Entities.Constants;

namespace MyCOLLAPI.Data;

public class DbSeeder
{
	public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		string[] roleNames = { UserRoles.Admin, UserRoles.Supplier, UserRoles.Client };

		foreach (var roleName in roleNames)
		{
			var roleExist = await roleManager.RoleExistsAsync(roleName);
			if (!roleExist)
			{
				await roleManager.CreateAsync(new IdentityRole(roleName));
			}
		}

		// Create Admin user
		var adminEmail = "admin@MyCOLLAPI.com";
		var adminUser = await userManager.FindByEmailAsync(adminEmail);

		if (adminUser == null)
		{
			var newAdmin = new ApplicationUser
			{
				UserName = adminEmail,
				Email = adminEmail,
				FullName = "Administrador do Sistema",
				EmailConfirmed = true,
				StateAccount = StateAccount.Active
			};

			// Create the admin user with a default password
			var createAdmin = await userManager.CreateAsync(newAdmin, "Admin@123");

			if (createAdmin.Succeeded)
			{
				// Assign Admin role to the user
				await userManager.AddToRoleAsync(newAdmin, UserRoles.Admin);
			}
		}
	}
}

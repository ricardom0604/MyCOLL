using Microsoft.AspNetCore.Identity;
using MyCOLL.Data.Data;
using MyCOLL.Shared.Constants;

namespace MyCOLL.Data;

public class DbSeeder
{
	public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
	{
		var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
		var userManager = serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();

		string[] roleNames = { UserRoles.Admin, UserRoles.Employee, UserRoles.Supplier, UserRoles.Client };

		foreach (var roleName in roleNames)
		{
			var roleExist = await roleManager.RoleExistsAsync(roleName);
			if (!roleExist)
			{
				await roleManager.CreateAsync(new IdentityRole(roleName));
			}
		}

		// Create Admin user
		var adminEmail = "admin@mycoll.com";
		var adminUser = await userManager.FindByEmailAsync(adminEmail);

		if (adminUser == null)
		{
			var newAdmin = new ApplicationUser
			{
				UserName = adminEmail,
				Email = adminEmail,
				FullName = "System Administrator",
				EmailConfirmed = true,
				StateAccount = StateAccount.Active,
				Nif = "000000000",
				Address = "Company Headquarters"
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

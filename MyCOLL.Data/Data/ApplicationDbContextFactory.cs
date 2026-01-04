using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace MyCOLL.Data.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		// Build configuration to read appsettings.json
		var configuration = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
			.AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: true)
			.Build();

		// Get connection string
		var connectionString = configuration.GetConnectionString("DefaultConnection");

		// Create DbContextOptionsBuilder
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
		optionsBuilder.UseSqlServer(connectionString);

		return new ApplicationDbContext(optionsBuilder.Options);
	}
}

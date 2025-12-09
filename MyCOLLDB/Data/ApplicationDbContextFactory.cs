using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using MyCOLLDB.Data;

namespace MyCOLLDB.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
	public ApplicationDbContext CreateDbContext(string[] args)
	{
		var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

		// Use the same connection string from appsettings.json
		var connectionString = "Server=127.0.0.1,1433;Database=mydb;User Id=SA;Password=YourPassword123!;Encrypt=False;TrustServerCertificate=True;MultipleActiveResultSets=True";

		optionsBuilder.UseSqlServer(connectionString, b => b.MigrationsAssembly("MyCOLLDB"));

		return new ApplicationDbContext(optionsBuilder.Options);
	}
}

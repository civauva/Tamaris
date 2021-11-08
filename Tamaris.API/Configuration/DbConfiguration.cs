using Microsoft.EntityFrameworkCore;
using Tamaris.DAL.DbContexts;


namespace Tamaris.API.Configuration
{
	public static class DbConfiguration
	{
		internal static void SetupDatabase(IConfiguration configuration, IServiceCollection services)
		{
			// register the DbContext on the container, getting the connection string from appSettings
			string connectionString = configuration["connectionStrings:sqlserverConnectionString"];
			services.AddDbContext<TamarisDbContext>(o => o.UseSqlServer(connectionString));
		}
	}
}
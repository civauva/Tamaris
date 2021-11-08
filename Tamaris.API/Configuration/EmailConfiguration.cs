using Microsoft.EntityFrameworkCore;
using Tamaris.API.Services.Email;
using Tamaris.API.Services.Email.Interfaces;
using Tamaris.DAL.DbContexts;


namespace Tamaris.API.Configuration
{
	public static class EmailConfiguration
	{
		internal static void SetupEmail(IConfiguration configuration, IServiceCollection services)
		{
			// register the DbContext on the container, getting the connection string from appSettings
			string connectionString = configuration["connectionStrings:sqlserverConnectionString"];
			services.AddDbContext<TamarisDbContext>(o => o.UseSqlServer(connectionString));

			var emailConfig = configuration
				.GetSection("EmailConfiguration")
				.Get<EmailParameters>();
			services.AddSingleton(emailConfig);
			services.AddScoped<IEmailSender, EmailSender>();
		}
	}
}
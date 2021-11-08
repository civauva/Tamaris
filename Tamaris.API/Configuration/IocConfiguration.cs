using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Tamaris.DAL.Interfaces;
using Tamaris.API.Services.Cache;
using Tamaris.API.Services.Cache.Interfaces;


namespace Tamaris.API.Configuration
{
	/// <summary>
	/// Inversion Of Control (Dependency Injection) start-up configuration.
	/// </summary>	
	public static class IocConfiguration
	{
		/// <summary>
		/// Configures the IoC/DI classes.
		/// </summary>
		/// <param name="configuration">The configuration.</param>
		/// <param name="services">The services.</param>
		internal static void SetupDependencies(IConfiguration configuration, IServiceCollection services)
		{
			services.AddScoped<ITamarisUnitOfWork, Tamaris.DAL.Repositories.TamarisUnitOfWork>();
			services.AddSingleton<ICacheService, InMemoryCacheService>();
		}
	}
}
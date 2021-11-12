using Microsoft.AspNetCore.Components.Authorization;
using Tamaris.Web.Services;
using Tamaris.Web.Services.DataService;

namespace Tamaris.Web.Configuration
{
    public class IdentityConfiguration
    {
        internal static void Configure(IServiceCollection services)
        {
            // Authentication/Authorization part
            services.AddAuthorizationCore();
            services.AddScoped<CustomStateProvider>();
            services.AddScoped<AuthenticationStateProvider, CustomStateProvider>();
            services.AddScoped<IAccountDataService, AccountDataService>();

            // This would have been alternative:

            //builder.Services.AddOidcAuthentication(options =>
            //{
            //    // Configure your authentication provider options here.
            //    // For more information, see https://aka.ms/blazor-standalone-auth
            //    builder.Configuration.Bind("Local", options.ProviderOptions);
            //});

            // But we are going the JWT way
        }
    }
}
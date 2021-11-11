using Tamaris.Web.Services;

namespace Tamaris.Web.Configuration
{
    public class IoCConfiguration
    {
        internal static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            string url = configuration["backEndServer"];

            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(/* builder.HostEnvironment.BaseAddress */ url) });
            services.AddScoped<IAdminDataService, AdminDataService>();
            services.AddScoped<IMessagesDataService, MessagesDataService>();
            services.AddScoped<IDialogService, DialogService>();
        }
    }
}
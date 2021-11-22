using Tamaris.Web.Services;
using Tamaris.Web.Services.DataService;

namespace Tamaris.Web.Configuration
{
    public class IoCConfiguration
    {
        internal static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            string url = configuration["backEndServer"];

            services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(/* builder.HostEnvironment.BaseAddress */ url), Timeout = new TimeSpan(0, 0, 30) }); // Set by default 30 seconds for the timeout
            services.AddTransient<IAdminDataService, AdminDataService>();
            services.AddTransient<IMessagesDataService, MessagesDataService>();
            services.AddScoped<IDialogService, DialogService>();
        }
    }
}
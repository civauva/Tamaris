using Tamaris.API.Hubs;

namespace Tamaris.API.Configuration
{
    public class SignalRConfiguration
    {

        public static void ConfigureSignalR(WebApplication app)
        {
            app.MapHub<MessageHub>("/messagehub");
        }

        internal static void SetupSignalR(IServiceCollection services)
        {
            services.AddSignalR();
        }
    }
}

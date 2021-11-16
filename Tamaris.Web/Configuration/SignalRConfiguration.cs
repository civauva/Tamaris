using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.SignalR.Client;

namespace Tamaris.Web.Configuration
{
    /// <summary>
    /// https://www.dotnetcurry.com/aspnet-core/realtime-app-using-blazor-webassembly-signalr-csharp9
    /// </summary>
    public class SignalRConfiguration
    {
        // We are just creating the connection, but not starting it yet
        // in order not to block the UI
        internal static void Configure(IServiceCollection services, IConfiguration configuration)
        {
            string url = configuration["backEndServer"];
            Uri uri = new(url);

            var baseUri = uri.GetLeftPart(System.UriPartial.Authority);
            // Uri hubUri = new(baseUri, "messagehub");
            // var hubUrl = hubUri.ToString();
            var hubUrl = baseUri + "/messagehub";

            services.AddSingleton<HubConnection>(sp =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();

                return new HubConnectionBuilder()
                .WithUrl(hubUrl)
                .WithAutomaticReconnect()
                .Build();
            });
        }
    }
}
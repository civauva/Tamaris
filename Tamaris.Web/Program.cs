using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.Modal;
using Blazored.LocalStorage;

using Tamaris.Web;
using Tamaris.Web.Configuration;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

var services = builder.Services;
var configuration = builder.Configuration;

// Configuring authentication/authorization
IdentityConfiguration.Configure(services);

// Configuring dependency injection
IoCConfiguration.Configure(services, configuration);

// Configure SignalR
SignalRConfiguration.Configure(services, configuration);

// Configuring AutoMapper
services.AddAutoMapper(typeof(Program));

// Registering Blazored packages
services.AddBlazoredLocalStorage();
services.AddBlazoredModal();



await builder.Build().RunAsync();
using System.Data;
using System.Linq;
using System.Collections.ObjectModel;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

using Serilog;
using Serilog.Events;


namespace Tamaris.API.Configuration
{
	public class LoggingConfiguration
	{
		internal static void SetupLogger()
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				.Enrich.FromLogContext()
				.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, buffered: false, shared: true)
				.CreateLogger();
		}


		internal static void SetupLogger(IConfiguration configuration)
		{
			Log.Logger = new LoggerConfiguration()
				.MinimumLevel.Verbose()
				.MinimumLevel.Override("Microsoft", LogEventLevel.Information)
				// Filter out ASP.NET Core infrastructre logs that are Information and below
				.MinimumLevel.Override("Microsoft.AspNetCore", LogEventLevel.Warning)
				.Enrich.FromLogContext()
				.WriteTo.File("logs/log-.txt", rollingInterval: RollingInterval.Day, buffered: false, shared: true)
				.WriteTo.Console()
				.CreateLogger();
		}


		// TODO:
		//internal static void EnrichLogging(IApplicationBuilder app)
		//{
		//	// TODO:
		//	app.UseSerilogRequestLogging(options =>
		//	{
		//		options.EnrichDiagnosticContext = PushSeriLogProperties;
		//	});
		//}


		//internal static void PushSeriLogProperties(IDiagnosticContext diagnosticContext, HttpContext httpContext)
		//{
		//	try
		//	{
		//		if (httpContext.User.Identity.IsAuthenticated && 
		//			httpContext.User.Claims.Any(i => i.Type == "UserId") && 
		//			int.TryParse(httpContext.User.Claims.FirstOrDefault(i => i.Type == "UserId").Value, out int UserId))
		//			diagnosticContext.Set("UserId", UserId);

		//		var ipAddress = httpContext.Connection.RemoteIpAddress.ToString();

		//		diagnosticContext.Set("UserIp", ipAddress.Equals("::1") ? "local" : ipAddress);
		//	}
		//	finally
		//	{

		//	}	
		//}
	}
}
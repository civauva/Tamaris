using Microsoft.OpenApi.Models;
using Serilog;
using Tamaris.API.Configuration;


const string CorsPolicyName = "TamarisCorsPolicy";

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var services = builder.Services;
services.AddControllers();

// Set configuration
var provider = services.BuildServiceProvider();
var configuration = builder.Configuration;

// Setup database
DbConfiguration.SetupDatabase(configuration, services);

// Setup identity
IdentityConfiguration.SetupIdentity(configuration, services);

// Setup email
EmailConfiguration.SetupEmail(configuration, services);

// Setup dependencies
IocConfiguration.SetupDependencies(configuration, services);

#region Cors
services.AddCors(options =>
{
	options.AddPolicy(CorsPolicyName,
		builder => builder.AllowAnyOrigin()
			.AllowAnyMethod()
			.AllowAnyHeader()
			.WithExposedHeaders("X-Pagination")
			);
	//.AllowCredentials());
});
#endregion Cors


// Register the Swagger generator
SwaggerConfiguration.RegisterSwagger(services);

// Configuring AutoMapper
services.AddAutoMapper(typeof(Program));

// Configure SeriLog
LoggingConfiguration.SetupLogger();
// TODO: services.AddLogging(loggingBuilder => loggingBuilder.AddSerilog(dispose: true));


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	// TODO: app.UseWebAssemblyDebugging();

	// Enable middleware to serve generated Swagger as a JSON endpoint
	SwaggerConfiguration.UseSwagger(app);
}
else
{
	app.UseExceptionHandler("/Error");
	// The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
	app.UseHsts();
}

app.UseCors(CorsPolicyName);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
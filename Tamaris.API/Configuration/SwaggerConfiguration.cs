using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;

using MicroElements.Swashbuckle.FluentValidation.AspNetCore;
using Swashbuckle.AspNetCore.Swagger;


namespace Tamaris.API.Configuration
{
    public static class SwaggerConfiguration
    {
		const string mainVersion = "v1";

        /// <summary>
        /// Register the Swagger generator, defining 1 or more Swagger documents
        /// </summary>
        internal static void RegisterSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
				c.SwaggerDoc("v1", CreateSwaggerInfo(VersionInfo, "Tamaris API"));

				// Specify our operation filter here - it is going to add standard header parameters in Swagger documents
				c.OperationFilter<CommonParameterOperationFilter>();


				// UseFullTypeNameInSchemaIds replacement for .NET Core - helps avoid problems where we have same type names but in different namespaces
				c.CustomSchemaIds(x => x.FullName);

				// Resolve potential action name conflicts
				c.ResolveConflictingActions(apiDescriptions => apiDescriptions.First());

				// Fetches XML controller comments into swagger documents
				var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
				var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
				c.IncludeXmlComments(xmlPath);
			});

			// Adds FluentValidationRules staff to Swagger. (Minimal configuration)
			services.AddFluentValidationRulesToSwagger();
		}

        private static OpenApiInfo CreateSwaggerInfo(string version, string title, string description = null)
        {
            return new OpenApiInfo
            {
                Version = version,
                Title = title,
                Description = description ?? "Restful API for Tamaris Project",
                // TermsOfService = "None",
                Contact = new OpenApiContact()
                {
                    Name = "John Doe",
                    Email = "dont@spam.me",
                    Url = new Uri("http://localhost/api/v1/")
                },
                License = new OpenApiLicense
                {
                    Name = "ABC",
                    Url = new Uri("http://www.unicef.com")
                }
            };
        }

        /// <summary>
        /// Enable middleware to serve generated Swagger as a JSON endpoint
        /// </summary>
        internal static void UseSwagger(IApplicationBuilder app)
        {
            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint(s).
            app.UseSwaggerUI(c =>
            {
				c.SwaggerEndpoint("/swagger/v1/swagger.json", "Tamaris API v1");
            });
        }



		#region Version number
		private static string versionInfo;
		internal static string VersionInfo
		{
			get
			{
				if (string.IsNullOrEmpty(versionInfo))
				{
					var date = Assembly.GetExecutingAssembly().GetBuildDateTime();
					versionInfo = mainVersion + (date.HasValue ? $".{date.Value.Year}.{date.Value.Month}.{date.Value.Day}" : "");
				}

				return versionInfo;
			}
		}


		/// <summary>
		/// Extension method that returns the creation time of the given assembly
		/// </summary>
		/// <param name="assembly"></param>
		/// <returns></returns>
		public static DateTimeOffset? GetBuildDateTime(this Assembly assembly)
		{
			var path = assembly.GetName().CodeBase;

			if (path.StartsWith(@"file:///"))
				path = path.Substring(8);

			if (File.Exists(path))
				return new DateTimeOffset(File.GetCreationTimeUtc(path));

			return null;
		}
		#endregion Version number
    }
}
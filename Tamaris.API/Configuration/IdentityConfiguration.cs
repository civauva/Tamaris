using Microsoft.AspNetCore.Identity;

using Tamaris.Entities.Admin;
using Tamaris.DAL.DbContexts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace Tamaris.API.Configuration
{
	public static class IdentityConfiguration
	{
		internal static void SetupIdentity(IConfiguration configuration, IServiceCollection services)
		{
			// Configuring EF Store for Identity issues
			services.AddIdentity<User, Role>(opt =>
            {
                opt.Password.RequiredLength = 7;
                opt.Password.RequireDigit = false;
                opt.Password.RequireUppercase = false;

                opt.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<TamarisDbContext>()
            .AddDefaultTokenProviders();

            services.ConfigureApplicationCookie(options =>
			{
				options.Cookie.HttpOnly = false;
				options.Events.OnRedirectToLogin = context =>
				{
					context.Response.StatusCode = 401;
					return Task.CompletedTask;
				};
			});


            var jwtSettings = configuration.GetSection("JwtSettings");
            services.AddAuthentication(opt =>
            {
                opt.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    ValidIssuer = jwtSettings.GetSection("validIssuer").Value,
                    ValidAudience = jwtSettings.GetSection("validAudience").Value,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.GetSection("securityKey").Value))
                };
            });
        }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

using hwwebapi.Core;
using hwwebapi.Values;

namespace hwwebapi {

    public class Startup {

        public IConfigurationRoot Configuration { get; }

        public Startup(IHostingEnvironment env) {
            // env.EnvironmentName is set via 'ASPNETCORE_ENVIRONMENT' environment variable
            // May need to 'export ASPNETCORE_ENVIRONMENT=development'?

            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables()
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false);

            Configuration = builder.Build();
        }

        // This method gets called by the runtime.
        // Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services) {
            // Add framework services.
            services.AddMvc();

            services.AddSingleton<IRepository<int, string>>(
                new SapientMySqlValuesRepository(
                    Configuration.GetSection("Database")["ConnectionString"]
                )
            );
        }

        // This method gets called by the runtime.
        // Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory loggerFactory) {

            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            ConfigureAuthentication(app);

            app.UseMvc();
        }

        private void ConfigureAuthentication(IApplicationBuilder app) {
            // https://auth0.com/docs/quickstart/backend/aspnet-core-webapi/02-authentication-hs256

            var configuration = Configuration.GetSection("Authentication");

            var domain = configuration["Domain"];
            var clientId = configuration["ClientId"];
            var clientSecret = Convert.FromBase64String(
                configuration["ClientSecret"].Replace('_', '/').Replace('-', '+')
            );

            app.UseJwtBearerAuthentication(
                new JwtBearerOptions {
                    TokenValidationParameters = {
                        ValidIssuer = $"https://{domain}/",
                        ValidAudience = clientId,
                        IssuerSigningKey = new SymmetricSecurityKey(clientSecret)
                    }
                }
            );
        }
    }
}

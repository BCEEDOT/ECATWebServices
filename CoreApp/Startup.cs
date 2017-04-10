using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Ecat.Data.Contexts;
using Ecat.Data.Models;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Http;
using Ecat.Web.Providers;
using System.IdentityModel.Tokens.Jwt;
using AspNet.Security.OpenIdConnect.Primitives;
using Ecat.Business.Repositories.Interface;
using Ecat.Business.Repositories;

namespace Ecat.Web
{
    public class Startup
    {
        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();
        }

        public IConfigurationRoot Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddAuthorization(options => {
                options.AddPolicy("ValuesUser",
                                    policy => policy.RequireClaim("Role", "Student"));
            });


            //Only for development
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                                       .AllowAnyMethod()
                                       .AllowAnyHeader()
                                       .AllowCredentials());
            });

            var connectionString = Configuration["DbConnection"];
            services.AddScoped(_ => new ValueContext(connectionString));
            //services.AddSingleton<IValueRepository, ValueRepository>();
            services.AddScoped<IValueRepository, ValueRepository>();
            services.AddScoped(_ => new ProductContext(connectionString));
            //services.AddSingleton<IProductRepository, ProductRepository>();
            services.AddScoped<IProductRepository, ProductRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            services.AddScoped(_ => new EcatContext(connectionString));
            services.AddScoped<IUserRepo, UserRepo>(); 
            // Add framework services.

           


            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole(Configuration.GetSection("Logging"));
            loggerFactory.AddDebug();

            //Only for development
            //Todo: Remove for production
            app.UseCors("CorsPolicy");

            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();


            app.UseJwtBearerAuthentication(new JwtBearerOptions {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true,
                Audience = "ecat_server",
                Authority  = "http://localhost:62187",
                RequireHttpsMetadata = false,
                TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters {
                    NameClaimType = OpenIdConnectConstants.Claims.Name,
                    RoleClaimType = OpenIdConnectConstants.Claims.Role
                }
                

            });

            app.UseOpenIdConnectServer(options =>
            {
                options.Provider = new AuthorizationProvider();
                options.ApplicationCanDisplayErrors = true;
                options.AllowInsecureHttp = true;
                options.AuthorizationEndpointPath = PathString.Empty;
                options.TokenEndpointPath = "/connect/token";
                options.AccessTokenHandler = new JwtSecurityTokenHandler
                {
                    InboundClaimTypeMap = new Dictionary<string, string>(),
                    OutboundClaimTypeMap = new Dictionary<string, string>()
                };

                //add certificate in production
                options.SigningCredentials.AddEphemeralKey();

            });

            app.UseMvc();
        }
    }
}

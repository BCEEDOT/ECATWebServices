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
using Microsoft.AspNetCore.Authorization;
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
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Reflection;

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
                options.AddPolicy("Faculty", policy => policy.RequireClaim("Role", "Faculty"));
                options.AddPolicy("Student", policy => policy.RequireClaim("Role", "Student"));
                options.AddPolicy("LMSAdmin", policy => policy.RequireClaim("Role", "ISA"));
                options.AddPolicy("LoggedInUser", policy => policy.Requirements.Add(new LoggedInUserRequirement()));
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
            services.AddScoped(_ => new EcatContext(connectionString));
            services.AddScoped<IUserRepo, UserRepo>();
            services.AddScoped<IStudentRepo, StudentRepo>();
            services.AddScoped<IFacultyRepo, FacultyRepo>();
            services.AddScoped<ILmsAdminCourseRepo, LmsAdminCourseRepo>();
            services.AddScoped<ILmsAdminGroupRepo, LmsAdminGroupRepo>();
            services.AddScoped<BbWsCnet, BbWsCnet>();
            services.AddScoped<IAuthorizationHandler, LoggedInUserPolicy>();

            //Controllers and AuthProvider need to have the httpContextAccessor injected
            //the accessor can be a singleton used across everything to access individual httpRequest contexts
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            // Add framework services.
            services.AddMvc().AddJsonOptions(options => {
                //JSON Serializer options
                //options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                //don't want to send back as camelcase because breeze is expecting pascal
                var resolver = options.SerializerSettings.ContractResolver;
                if (resolver != null)
                {
                    var res = resolver as DefaultContractResolver;
                    res.NamingStrategy = null;
                }
                //ReferenceLoopHandling.Ignore stops serialization of self-referencing objects (ie Person > ProfileFaculty > Person > etc)
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                //sends back $type on all objects, but not collections
                options.SerializerSettings.TypeNameHandling = TypeNameHandling.Objects;
                //adds $id and $ref, making the formatter properly read object references
                options.SerializerSettings.PreserveReferencesHandling = PreserveReferencesHandling.Objects;
            });
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
                //OpenIdConnectServer is a singleton, so anything injected has a singleton lifetime (ie repo and context) the singleton accessor can be used to access the current httpContext
                options.Provider = new AuthorizationProvider(app.ApplicationServices.GetService<IHttpContextAccessor>());
                options.ApplicationCanDisplayErrors = true;
                options.AllowInsecureHttp = true;
                options.AuthorizationEndpointPath = PathString.Empty;
                options.TokenEndpointPath = "/connect/token";

                //TODO: Update token lifetime before release
                options.AccessTokenLifetime = TimeSpan.FromHours(1);
                options.IdentityTokenLifetime = TimeSpan.FromHours(1);
                options.AccessTokenHandler = new JwtSecurityTokenHandler
                {
                    InboundClaimTypeMap = new Dictionary<string, string>(),
                    OutboundClaimTypeMap = new Dictionary<string, string>()
                };

                //TODO: Replace with real certifcate for production
                options.SigningCredentials.AddEphemeralKey();

                //options.SigningCredentials.AddCertificate(
                //    assembly: typeof(Startup).GetTypeInfo().Assembly,
                //    resource: "Ecat.Web.EcatCertificate.pfx",
                //    password: "ecatisawesome");

            });

            app.UseStaticFiles();
            app.UseMvc();
        }
    }
}

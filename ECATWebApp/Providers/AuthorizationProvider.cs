using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.Extensions.DependencyInjection;
using Ecat.Data.Models.User;
using Ecat.Data.Contexts;
using Breeze.Persistence.EF6;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;
using Ecat.Business.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;


namespace Ecat.Web.Providers
{
    public sealed class AuthorizationProvider : OpenIdConnectServerProvider
    {

        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context) {
            if (!context.Request.IsPasswordGrantType()) {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only the resource owner password credentials grant is accepted by this authorization server"
               );

                await Task.FromResult(context.IsValidated);
            }

            context.Skip();

            await Task.FromResult(context.IsValidated);

        }

        public override async Task HandleTokenRequest(HandleTokenRequestContext context) {       

            if (context.Request.IsPasswordGrantType()) {
                var username = context.Request.Username;
                var password = context.Request.Password;

                //TODO: Fix so it reads connection string from app.config -- injecting not working due to newing in startup
                //var ecatCtx = new EcatContext();
                var ecatCtx = new EcatContext("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ecatlocaldev;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

                //get the person with their security entry and faculty profile if they have one
                var person = await ecatCtx.People.Where(p => p.Email == username)
                    .Include(p => p.Security)
                    .Include(p => p.Faculty)
                    .SingleOrDefaultAsync();

                if (person == null)
                {
                    throw new UnauthorizedAccessException("Username not found");
                }

                var validPass = PasswordHash.ValidatePassword(password, person.Security.PasswordHash);
                if (!validPass) {
                    throw new UnauthorizedAccessException("Invalid Username/Password Combination");
                }

                var identity = new ClaimsIdentity(
                        OpenIdConnectServerDefaults.AuthenticationScheme, OpenIdConnectConstants.Claims.Name, OpenIdConnectConstants.Claims.Role);

                //required for ASOS
                identity.AddClaim(OpenIdConnectConstants.Claims.Subject, person.PersonId.ToString());

                switch (person.MpInstituteRole)
                {
                    case "ECAT_05_Student":
                        identity.AddClaim(OpenIdConnectConstants.Claims.Role, "Student", OpenIdConnectConstants.Destinations.AccessToken);
                        break;
                    case "ECAT_04_Faculty":
                        identity.AddClaim(OpenIdConnectConstants.Claims.Role, "Faculty", OpenIdConnectConstants.Destinations.AccessToken);
                        //ISA isn't seperate from faculty, just a flag
                        if (person.Faculty.IsCourseAdmin)
                        {
                            identity.AddClaim(OpenIdConnectConstants.Claims.Role, "ISA", OpenIdConnectConstants.Destinations.AccessToken);
                        }
                        break;
                    default:
                        identity.AddClaim(OpenIdConnectConstants.Claims.Role, "Student", OpenIdConnectConstants.Destinations.AccessToken);
                        break;
                }

                //identity token information
                identity.AddClaim("lastName", person.LastName, OpenIdConnectConstants.Destinations.IdentityToken);
                identity.AddClaim("firstName", person.FirstName, OpenIdConnectConstants.Destinations.IdentityToken);
                identity.AddClaim("email", person.Email, OpenIdConnectConstants.Destinations.IdentityToken);
                identity.AddClaim("mpAffiliation", person.MpAffiliation, OpenIdConnectConstants.Destinations.IdentityToken);
                identity.AddClaim("mpComponent", person.MpComponent, OpenIdConnectConstants.Destinations.IdentityToken);
                identity.AddClaim("mpPaygrade", person.MpPaygrade, OpenIdConnectConstants.Destinations.IdentityToken);
                identity.AddClaim("mpGender", person.MpGender, OpenIdConnectConstants.Destinations.IdentityToken);
                identity.AddClaim("mpInstituteRole", person.MpInstituteRole, OpenIdConnectConstants.Destinations.IdentityToken);

                var ticket = new AuthenticationTicket(
                    new ClaimsPrincipal(identity), new AuthenticationProperties(), context.Options.AuthenticationScheme);

                ticket.Properties.IssuedUtc = DateTime.Now;
                ticket.Properties.ExpiresUtc = DateTime.Now.Add(TimeSpan.FromHours(1));

                ticket.SetScopes(OpenIdConnectConstants.Scopes.OpenId);
                ticket.SetResources("ecat_server");
                context.Validate(ticket);

                await Task.FromResult(context.IsValidated);
            }
           
        }

        //public override Task ApplyTokenResponse(ApplyTokenResponseContext context)
        //{
        //    context.Response["CUSTOM"] = "CUSTOMVALUE";

        //    return base.ApplyTokenResponse(context);
        //}

    }
}

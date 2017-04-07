using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.Extensions.DependencyInjection;
using ECATDataLib.Models;
using ECATDataLib.Contexts;
using Breeze.Persistence.EF6;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;

namespace CoreApp.Providers
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
                //return;
            }

            context.Skip();

            await Task.FromResult(context.IsValidated);
            //return;

        }

        public override async Task HandleTokenRequest(HandleTokenRequestContext context) {

            Console.WriteLine("test");
         

            if (context.Request.IsPasswordGrantType()) {
                var username = context.Request.Username;
                var password = context.Request.Password;

                if (username == "test" && password == "password") {

                    var identity = new ClaimsIdentity(
                        OpenIdConnectServerDefaults.AuthenticationScheme, OpenIdConnectConstants.Claims.Name, OpenIdConnectConstants.Claims.Role);

                    identity.AddClaim(OpenIdConnectConstants.Claims.Subject, "[1]");
                    //identity.AddClaim("username", "test", OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);
                    identity.AddClaim("User", "Student", OpenIdConnectConstants.Destinations.AccessToken, OpenIdConnectConstants.Destinations.IdentityToken);

                    var ticket = new AuthenticationTicket(
                        new ClaimsPrincipal(identity), new AuthenticationProperties(), context.Options.AuthenticationScheme);

                    //ticket.SetScopes(
                    //    "test", "test2", "test3"
                    //);

                    ticket.SetResources("resource_server");


                    context.Validate(ticket);
                   
                }

                await Task.FromResult(context.IsValidated);
                //return;
            }
           
        }

    }
}

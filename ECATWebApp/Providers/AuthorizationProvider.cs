﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using AspNet.Security.OpenIdConnect.Primitives;
using AspNet.Security.OpenIdConnect.Server;
using Microsoft.Extensions.DependencyInjection;
using Ecat.Data.Models.User;
using Ecat.Data.Contexts;
using Ecat.Business.Utilities;
using Ecat.Business.Repositories.Interface;
using Breeze.Persistence.EF6;
using AspNet.Security.OpenIdConnect.Extensions;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Authentication;
using System.Security.Claims;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lti1;


namespace Ecat.Web.Providers
{
    public sealed class AuthorizationProvider : OpenIdConnectServerProvider
    {
        private IUserRepo userRepo;

        public AuthorizationProvider(IUserRepo repo)
        {
            userRepo = repo;
        }

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

            var username = "";
            var password = "";
            var person = new Person();
            var isLti = false;

            //Method comes from Lti extensions, so is only on the request if it is LTI, so anything else throws an error. figure out a nicer way to do this
            try { isLti = context.HttpContext.Request.IsAuthenticatedWithLti(); }
            catch { isLti = false; }

            var isPassword = context.Request.IsPasswordGrantType();
            
            if (!isLti && !isPassword)
            {
                context.Reject(error: OpenIdConnectConstants.Errors.UnsupportedGrantType, description: "Not a valid grant type request");
                return;
            }

            if (isLti)
            {
                var ltiRequest = await context.HttpContext.Request.ParseLtiRequestAsync();
                person = await userRepo.ProcessLtiUser(ltiRequest);
            }

            if (isPassword)
            {
                username = context.Request.Username;
                password = context.Request.Password;

                person = await userRepo.GetUserInfoByEmail(username);
            }

            if (person == null)
            {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidGrant, description: "User not found");
                return;
            }

            if (isPassword)
            {
                var validPass = PasswordHash.ValidatePassword(password, person.Security.PasswordHash);
                if (!validPass)
                {
                    context.Reject(error: OpenIdConnectConstants.Errors.AccessDenied, description: "Invalid Credentials");
                    return;
                }
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
            identity.AddClaim("registrationComplete", person.RegistrationComplete.ToString(), OpenIdConnectConstants.Destinations.IdentityToken);

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
}

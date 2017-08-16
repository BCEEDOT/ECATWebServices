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

using LtiLibrary.AspNetCore.Extensions;
using LtiLibrary.NetCore.Extensions;
using LtiLibrary.NetCore.Lti1;
using Ecat.Business.Repositories.Interface;
using Ecat.Data.Static;

namespace Ecat.Web.Providers
{
    public sealed class AuthorizationProvider : OpenIdConnectServerProvider
    {

        //private IUserRepo userRepo;
        //private EcatContext ctx;
        private string connection;

        //public AuthorizationProvider(EcatContext context)
        //{
        //    ctx = context;
        //}

        //public AuthorizationProvider(IUserRepo repo)
        //{
        //    userRepo = repo;
        //}

        public AuthorizationProvider(string connString)
        {
            connection = connString;
        }

        public override async Task ValidateTokenRequest(ValidateTokenRequestContext context)
        {
            if (!context.Request.IsPasswordGrantType())
            {
                context.Reject(
                    error: OpenIdConnectConstants.Errors.UnsupportedGrantType,
                    description: "Only the resource owner password credentials grant is accepted by this authorization server"
               );

                await Task.FromResult(context.IsValidated);
            }

            context.Skip();

            await Task.FromResult(context.IsValidated);

        }

        public override async Task HandleTokenRequest(HandleTokenRequestContext context)
        {

            //if (context.Request.IsPasswordGrantType()) {
            //var username = context.Request.Username;
            //var password = context.Request.Password;

            //TODO: Fix so it reads connection string from app.config -- injecting not working due to newing in startup
            var ecatCtx = new EcatContext(connection);
            //var ecatCtx = new EcatContext("Data Source=(localdb)\\MSSQLLocalDB;Initial Catalog=ecatlocaldev;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=True;ApplicationIntent=ReadWrite;MultiSubnetFailover=False");

            //get the person with their security entry and faculty profile if they have one
            //var person = await ecatCtx.People.Where(p => p.Email == username)
            //.Include(p => p.Security)
            //.Include(p => p.Faculty)
            //.SingleOrDefaultAsync();

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
                try
                {
                    //person = await userRepo.ProcessLtiUser(ltiRequest);
                    person = await ProcessLtiUser(ecatCtx, ltiRequest);
                }
                catch (InvalidEmailException ex)
                {
                    context.Reject("Invalid Credentials", ex.Message + "\n\n Please update your email address in both the LMS and AU Portal to use ECAT.");
                    await Task.FromException(ex);
                }
                catch (UserUpdateException)
                {
                    context.Reject("Update Error", "There was an error updating your account with the information from the LMS. Please try again.");
                    return;
                }
            }

            if (isPassword)
            {
                username = context.Request.Username;
                password = context.Request.Password;
                //person = await userRepo.GetUserInfoByEmail(username);
                person = await ecatCtx.People.Where(p => p.Email == username)
                .Include(p => p.Security)
                .Include(p => p.Faculty)
                .SingleOrDefaultAsync();
            }

            if (person == null)
            {
                context.Reject(error: OpenIdConnectConstants.Errors.InvalidGrant, description: "Username not found");
                return;
            }

            if (isPassword)
            {
                var validPass = PasswordHash.ValidatePassword(password, person.Security.PasswordHash);
                if (!validPass)
                {
                    context.Reject(
                        error: OpenIdConnectConstants.Errors.InvalidGrant,
                        description: "Invalid Credentials");
                    return;
                }
            }

            //clean connection
            ecatCtx.Dispose();

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
                    else
                    {
                        identity.AddClaim(OpenIdConnectConstants.Claims.Role, "notISA", OpenIdConnectConstants.Destinations.AccessToken);
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

            //these didn't actually do anything for us it seems
            //ticket.Properties.IssuedUtc = DateTime.Now;
            //ticket.Properties.ExpiresUtc = DateTime.Now.AddHours(6);

            ticket.SetScopes(OpenIdConnectConstants.Scopes.OpenId);
            ticket.SetResources("ecat_server");
            context.Validate(ticket);

            await Task.FromResult(context.IsValidated);
        }

        public async Task<Person> ProcessLtiUser(EcatContext ctx, LtiRequest parsedRequest)
        {
            var user = await ctx.People
             .Include(s => s.Security)
             .Include(s => s.Faculty)
             .Include(s => s.Student)
             .SingleOrDefaultAsync(person => person.BbUserId == parsedRequest.UserId);

            var emailChecker = new ValidEmailChecker();
            if (user != null)
            {
                if (user.Email.ToLower() != parsedRequest.LisPersonEmailPrimary.ToLower())
                {
                    if (!emailChecker.IsValidEmail(parsedRequest.LisPersonEmailPrimary))
                    {
                        throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is not a valid email address.");

                    }
                    var uniqueEmail = await ctx.People.CountAsync(per => per.Email.ToLower() == parsedRequest.LisPersonEmailPrimary.ToLower()) == 0;
                    if (!uniqueEmail)
                    //if (!await UniqueEmailCheck(parsedRequest.LisPersonEmailPrimary))
                    {
                        throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is already being used in ECAT.");
                    }
                }
                user.ModifiedById = user.PersonId;
            }
            else
            {
                if (!emailChecker.IsValidEmail(parsedRequest.LisPersonEmailPrimary))
                {
                    throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is not a valid email address.");
                }
                var uniqueEmail = await ctx.People.CountAsync(per => per.Email.ToLower() == parsedRequest.LisPersonEmailPrimary.ToLower()) == 0;
                if (!uniqueEmail)
                //if (!await UniqueEmailCheck(parsedRequest.LisPersonEmailPrimary))
                {
                    throw new InvalidEmailException("The email address associated with your LMS account (" + parsedRequest.LisPersonEmailPrimary + ") is already being used in ECAT.");
                }

                user = new Person
                {
                    IsActive = true,
                    MpGender = MpGender.Unk,
                    MpAffiliation = MpAffiliation.Unk,
                    MpComponent = MpComponent.Unk,
                    MpPaygrade = MpPaygrade.Unk,
                    MpInstituteRole = MpInstituteRoleId.Undefined,
                    RegistrationComplete = false
                };

                ctx.People.Add(user);
            }

            var userIsCourseAdmin = false;

            switch (parsedRequest.Parameters["Roles"].ToLower())
            {
                case "instructor":
                    userIsCourseAdmin = true;
                    user.MpInstituteRole = MpInstituteRoleId.Faculty;
                    break;
                case "teachingassistant":
                    user.MpInstituteRole = MpInstituteRoleId.Faculty;
                    break;
                case "contentdeveloper":
                    user.MpInstituteRole = MpInstituteRoleId.Designer;
                    break;
                default:
                    user.MpInstituteRole = MpInstituteRoleId.Student;
                    break;
            }

            switch (user.MpInstituteRole)
            {
                case MpInstituteRoleId.Faculty:
                    user.Faculty = user.Faculty ?? new ProfileFaculty();
                    user.Faculty.IsCourseAdmin = userIsCourseAdmin;
                    user.Faculty.AcademyId = parsedRequest.Parameters["custom_ecat_school"];
                    break;
                case MpInstituteRoleId.Designer:
                    //user.Designer = user.Designer ?? new ProfileDesigner();
                    //user.Designer.AssociatedAcademyId = parsedRequest.Parameters["custom_ecat_school"];
                    break;
                default:
                    user.Student = user.Student ?? new ProfileStudent();
                    break;
            }

            user.IsActive = true;
            user.Email = parsedRequest.LisPersonEmailPrimary.ToLower();
            user.LastName = parsedRequest.LisPersonNameFamily;
            user.FirstName = parsedRequest.LisPersonNameGiven;
            user.BbUserId = parsedRequest.UserId;
            user.ModifiedDate = DateTime.Now;

            if (await ctx.SaveChangesAsync() > 0)
            {
                return user;
            }

            throw new UserUpdateException("Save User Changes did not succeed!");
        }

    }

}

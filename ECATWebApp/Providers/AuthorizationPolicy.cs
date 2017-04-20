using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using AspNet.Security.OpenIdConnect.Primitives;
using Ecat.Data.Contexts;
using Ecat.Business.Repositories.Interface;
using Microsoft.AspNetCore.Http;
using Ecat.Web.Controllers;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc;

namespace Ecat.Web.Providers
{
    public class LoggedInUserRequirement : IAuthorizationRequirement { }

    public class LoggedInUserPolicy : AuthorizationHandler<LoggedInUserRequirement>
    {

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, LoggedInUserRequirement requirement)
        {
            //Check to see if the token can be read and has a subject claim on it
            var principal = context.User as ClaimsPrincipal;
            var subClaim = principal.Claims.FirstOrDefault(claim => claim.Type == OpenIdConnectConstants.Claims.Subject);
            if (subClaim != null && int.TryParse(subClaim.Value, out int result)) //&& subClaim.Issuer == )
            {
                context.Succeed(requirement);
            }
            else
            {
                context.Fail();
            }

            return Task.FromResult(context.HasSucceeded);
        }
    }
}

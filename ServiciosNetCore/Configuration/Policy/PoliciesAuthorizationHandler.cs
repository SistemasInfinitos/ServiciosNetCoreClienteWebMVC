using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServiciosNetCore.Configuration.Policy
{
    public class PoliciesAuthorizationHandler : AuthorizationHandler<PersonaRequireClaim>, IAuthorizationHandler
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, PersonaRequireClaim requirement)
        {
            bool ok = false;
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.FromResult(0);
            }
            else
            {
                context.Succeed(requirement);
            }

            return Task.FromResult(0);
        }
    }
}

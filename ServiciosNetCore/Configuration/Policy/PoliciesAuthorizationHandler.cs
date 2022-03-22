using Microsoft.AspNetCore.Authorization;
using ServiciosNetCore.ModelsAPI.Comun;
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
            /*Aqui podriamos agregas las politicas, actualmente solo valida IsAuthenticated el cual ya fue validado por [Authorize] */
            ResponseApp data = new ResponseApp()
            {
                mensaje = "Ups!. Tu solicitud no pudo ser autorizada, verifica tus credenciales!",
                ok = false
            };
            if (context.User == null || !context.User.Identity.IsAuthenticated)
            {
                context.Fail();
                return Task.FromResult(data);
            }
            else
            {
                data.ok = true;
                data.mensaje= "Succeed";
                context.Succeed(requirement);
            }

            return Task.FromResult(data);
        }
    }
}

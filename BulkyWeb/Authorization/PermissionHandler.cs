using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace BulkyWeb.Authorization
{
    public class PermissionHandler : AuthorizationHandler<PermissionRequirement>
    {
        protected override Task HandleRequirementAsync(
            AuthorizationHandlerContext context,
            PermissionRequirement requirement)
        {
            var httpContext = context.Resource as HttpContext;

            var action = httpContext?
                .GetEndpoint()?
                .Metadata
                .GetMetadata<ControllerActionDescriptor>();

            if (action == null)
                return Task.CompletedTask;

            var requiredPermission =
                $"{action.ControllerName}.{action.ActionName}";

            var hasPermission = context.User.Claims.Any(c =>
                c.Type == "permission" &&
                c.Value == requiredPermission);

            if (hasPermission)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }



}

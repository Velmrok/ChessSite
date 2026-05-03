using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace backend.Policies;
public class OwnerOrAdminRequirement : IAuthorizationRequirement { }

public class OwnerOrAdminHandler : AuthorizationHandler<OwnerOrAdminRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        OwnerOrAdminRequirement requirement)
    {
        if (context.Resource is not HttpContext httpContext)
        {
            context.Fail();
            return Task.CompletedTask;
        }

        var userNickname = context.User.FindFirst("nickname")?.Value;
        var isAdmin = context.User.FindFirst(ClaimTypes.Role)?.Value == "Admin";

        if (isAdmin)
        {
            context.Succeed(requirement);
            return Task.CompletedTask;
        }

        var routeNickname = httpContext.GetRouteValue("nickname")?.ToString();

        if (userNickname is not null
            && routeNickname is not null
            && userNickname == routeNickname)
        {
            context.Succeed(requirement);
        }
        else
        {
            context.Fail();
        }

        return Task.CompletedTask;
    }
}
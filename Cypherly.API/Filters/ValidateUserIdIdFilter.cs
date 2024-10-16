using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace Cypherly.API.Filters;

public class ValidateUserIdIdFilter(ILogger<ValidateUserIdIdFilter> logger) : IValidateUserIdFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var userGuidClaim = context.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        
        if (userGuidClaim == null || !Guid.TryParse(userGuidClaim, out var userGuid))
        {
            context.Result = new UnauthorizedResult();
            return;
        }
        
        if (context.ActionArguments.TryGetValue("command", out var command))
        {
            var commandType = command.GetType();
            var userIdProperty = commandType.GetProperty("Id");
            
            if (userIdProperty == null)
            {
                context.Result = new BadRequestObjectResult("Command does not have a UserId property");
                return;
            }
            
            var userId = userIdProperty.GetValue(command);
            if (userId is Guid commandUserId && commandUserId != userGuid)
            {
                logger.LogCritical("User {UserGuid} attempted to access user {CommandUserId}", userGuid, commandUserId);
                context.Result = new UnauthorizedResult();
                return;
            }
        }
        
        await next();
    }
}
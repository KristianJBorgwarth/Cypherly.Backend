using Cypherly.API.Filters;
using Microsoft.AspNetCore.Mvc.Filters;

namespace TestUtilities.Authentication;

public class MockValidateUserIdIdFilter : IValidateUserIdFilter
{
    public new async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();
    }
}
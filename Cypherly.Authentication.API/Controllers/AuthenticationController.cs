using Cypherly.Authentication.Application.Features.Authentication.Commands.Login;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cypherly.Authentication.Controllers;

[Route("api/[controller]")]
public class AuthenticationController(ISender sender) : BaseController
{
    [HttpPost]
    [Route("login")]
    [ProducesResponseType(typeof(LoginDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }
}
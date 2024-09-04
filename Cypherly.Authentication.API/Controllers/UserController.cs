using Cypherly.Authentication.Application.Features.User.Commands.Create;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cypherly.Authentication.Controllers;

[Route("api/[controller]")]
public class UserController(ISender sender) : BaseController
{
    [HttpPost]
    [Route("")]
    [ProducesResponseType(typeof(CreateUserDto),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody]CreateUserCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }
}
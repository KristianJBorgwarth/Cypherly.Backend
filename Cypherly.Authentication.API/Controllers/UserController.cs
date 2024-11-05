using Cypherly.Authentication.Application.Features.User.Commands.Create;
using Cypherly.Authentication.Application.Features.User.Commands.Delete;
using Cypherly.Authentication.Application.Features.User.Commands.Update.ResendVerificationCode;
using Cypherly.Authentication.Application.Features.User.Commands.Update.Verify;
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
    
    [HttpDelete]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromQuery]DeleteUserCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpPut]
    [Route("verify")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Verify([FromBody]VerifyUserCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpPut]
    [Route("resend-verification")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ResendVerification([FromBody]GenerateAccountVerificationCodeCommand codeCommand)
    {
        var result = await sender.Send(codeCommand);
        return result.Success ? Ok() : Error(result.Error);
    }
}
using Cypherly.Authentication.Application.Features.Authentication.Commands.Login;
using Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyNonce;
using Cypherly.Authentication.Application.Features.Authentication.Queries.GetNonce;
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

    [HttpGet]
    [Route("nonce")]
    [ProducesResponseType(typeof(GetNonceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetNonce([FromQuery] GetNonceQuery query)
    {
        var result = await sender.Send(query);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpPost]
    [Route("verify-nonce")]
    [ProducesResponseType(typeof(VerifyNonceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> VerifyNonce([FromBody] VerifyNonceCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }
}
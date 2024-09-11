﻿using Cypherly.Authentication.Application.Features.Claim.Commands.Create.Claim;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cypherly.Authentication.Controllers;

[Route("api/[controller]")]
public class ClaimController(ISender sender) : BaseController
{
    [HttpPost]
    [Route("")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody]CreateClaimCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }
}
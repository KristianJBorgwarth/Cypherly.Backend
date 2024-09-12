using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Create.Friendship;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cypherly.UserManagement.API.Controllers;

[Route("api/[controller]")]
public class UserProfileController(ISender sender) : BaseController
{
    [HttpPost("friendship")]
    [ProducesResponseType(typeof(CreateFriendshipDto),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateFriendship([FromBody] CreateFriendshipCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }
}
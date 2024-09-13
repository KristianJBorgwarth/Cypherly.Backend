using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Create.Friendship;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cypherly.UserManagement.API.Controllers;

[Route("api/[controller]")]
public class UserProfileController(ISender sender) : BaseController
{
    [HttpPost("friendship")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateFriendship([FromBody] CreateFriendshipCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpGet("")]
    [ProducesResponseType(typeof(GetUserProfileDto),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserProfile([FromQuery] GetUserProfileQuery query)
    {
        var result = await sender.Send(query);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpGet("friends")]
    [ProducesResponseType(typeof(GetFriendsDto),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFriends([FromQuery] GetFriendsQuery query)
    {
        var result = await sender.Send(query);

        if (result.Success is false) return Error(result.Error);

        return result.Value.Count > 0 ? Ok(result.Value) : NoContent();
    }

    [HttpPut("profile-picture")]
    [ProducesResponseType(typeof(UpdateUserProfilePictureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfilePicture([FromQuery] UpdateUserProfilePictureCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }
}
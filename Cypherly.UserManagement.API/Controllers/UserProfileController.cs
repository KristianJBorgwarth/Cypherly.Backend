﻿using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Create.Friendship;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Delete.Friendship;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.AcceptFriendship;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.DisplayName;
using Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.ProfilePicture;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetFriends;
using Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfile;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cypherly.UserManagement.API.Controllers;

[Route("api/[controller]")]
public class UserProfileController(ISender sender) : BaseController
{

    [HttpGet("")]
    [ProducesResponseType(typeof(GetUserProfileDto),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetUserProfile([FromQuery] GetUserProfileQuery query)
    {
        var result = await sender.Send(query);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpPut("profile-picture")]
    [ProducesResponseType(typeof(UpdateUserProfilePictureDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetProfilePicture([FromForm] UpdateUserProfilePictureCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }

    [HttpPut("displayname")]
    [ProducesResponseType(typeof(UpdateUserProfileDisplayNameDto) ,StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> UpdateDisplayName([FromBody] UpdateUserProfileDisplayNameCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok(result.Value) : Error(result.Error);
    }


    [HttpPost("friendship")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> CreateFriendship([FromBody] CreateFriendshipCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }

    [HttpGet("friendships")]
    [ProducesResponseType(typeof(GetFriendsDto),StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFriends([FromQuery] GetFriendsQuery query)
    {
        var result = await sender.Send(query);

        if (result.Success is false) return Error(result.Error);

        return result.Value.Count > 0 ? Ok(result.Value) : NoContent();
    }
    [HttpPut("friendship")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> AcceptFriendship([FromBody] AcceptFriendshipCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }
    
    [HttpDelete("friendship")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> RemoveFriendship([FromBody] DeleteFriendshipCommand command)
    {
        var result = await sender.Send(command);
        return result.Success ? Ok() : Error(result.Error);
    }
}
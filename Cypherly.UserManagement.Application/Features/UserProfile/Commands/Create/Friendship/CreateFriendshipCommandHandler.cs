using AutoMapper;
using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.Domain.ValueObjects;
using Cypherly.UserManagement.Application.Contracts;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Create.Friendship;

public class CreateFriendshipCommandHandler(
    IUserProfileRepository userProfileRepository,
    IMapper mapper,
    IUnitOfWork unitOfWork,
    ILogger<CreateFriendshipCommandHandler> logger)
    : ICommandHandler<CreateFriendshipCommand, CreateFriendshipDto>
{
    public async Task<Result<CreateFriendshipDto>> Handle(CreateFriendshipCommand request, CancellationToken cancellationToken)
    {
        try
        {
            throw new NotImplementedException();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error in {Command}", nameof(CreateFriendshipCommand));
            return Result.Fail<CreateFriendshipDto>(Errors.General.UnspecifiedError("Exception occured while attempting to create friendship"));
        }
    }
}
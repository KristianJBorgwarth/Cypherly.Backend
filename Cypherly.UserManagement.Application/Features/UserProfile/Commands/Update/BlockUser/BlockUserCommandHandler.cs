using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Application.Features.UserProfile.Commands.Update.BlockUser;

public class BlockUserCommandHandler(
    IUserProfileRepository profileRepository,
    IUserProfileService profileService,
    IUnitOfWork uow,
    ILogger<BlockUserCommandHandler> logger)
    : ICommandHandler<BlockUserCommand>
{
    public Task<Result> Handle(BlockUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
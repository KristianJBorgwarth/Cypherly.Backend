using Cypherly.Application.Abstractions;
using Cypherly.Application.Contracts.Repository;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Services.User;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Commands.Delete;

public class DeleteUserCommandHandler(
    IUserRepository userRepository,
    IUnitOfWork unitOfWork,
    IUserLifeCycleService  userLifeCycleService,
    ILogger<DeleteUserCommandHandler> logger)
    : ICommandHandler<DeleteUserCommand>
{
    public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.Id);
            if (user is null)
            {
                logger.LogError("User not found with id {Id} during delete process", request.Id);
                return Result.Fail(Errors.General.NotFound(request.Id));
            }

            userLifeCycleService.SoftDelete(user);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return Result.Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }
}
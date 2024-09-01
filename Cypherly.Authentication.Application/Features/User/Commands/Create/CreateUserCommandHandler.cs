using Cypherly.Application.Abstractions;
using Cypherly.Domain.Common;
using MediatR;

namespace Cypherly.Authentication.Application.Features.User.Commands.Create;

public class CreateUserCommandHandler : ICommandHandler<CreateUserCommand, CreateUserDto>
{
    public Task<Result<CreateUserDto>> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
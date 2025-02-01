using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Connect;

public class ConnectCommandValidator : AbstractValidator<ConnectCommand>
{
    public ConnectCommandValidator()
    {
        RuleFor(x => x.ClientId)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(ConnectCommand.ClientId)).Message);

        RuleFor(x => x.TransientId)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(ConnectCommand.TransientId)).Message);
    }
}
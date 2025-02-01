using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Connect;

public class ConnectCommandValidator : AbstractValidator<ConnectCommand>
{
    public ConnectCommandValidator()
    {
        RuleFor(x => x.CliendId)
            .NotEmpty()
            .WithMessage(Errors.General.ValueIsEmpty(nameof(ConnectCommand.CliendId)).Message);
    }
}
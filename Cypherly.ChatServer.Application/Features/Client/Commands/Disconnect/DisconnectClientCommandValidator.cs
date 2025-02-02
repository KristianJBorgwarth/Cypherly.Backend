using FluentValidation;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;

public class DisconnectCommandValidator : AbstractValidator<DisconnectClientCommand>
{
    public DisconnectCommandValidator()
    {

    }
}
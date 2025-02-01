using FluentValidation;

namespace Cypherly.ChatServer.Application.Features.Client.Commands.Disconnect;

public class DisconnectCommandValidator : AbstractValidator<DisconnectCommand>
{
    public DisconnectCommandValidator()
    {
        
    }
}
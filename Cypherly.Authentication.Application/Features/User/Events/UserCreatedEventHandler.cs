using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Domain.Events.User;

namespace Cypherly.Authentication.Application.Features.User.Events;

public class UserCreatedEventHandler(
    IUserRepository userRepository)
    : IDomainEventHandler<UserCreatedEvent>
{
    public Task Handle(UserCreatedEvent notification, CancellationToken cancellationToken)
    {
        Console.Write("User created: ");
        Console.WriteLine(notification.Email);
        return Task.CompletedTask;
    }
}
﻿using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IUserService
{
    Result<Aggregates.User> CreateUser(string email, string password);
}

public class UserService : IUserService
{
    public Result<Aggregates.User> CreateUser(string email, string password)
    {
        var emailResult = Email.Create(email);
        if (emailResult.Success is false)
            return Result.Fail<Aggregates.User>(emailResult.Error);

        var pwResult = Password.Create(password);
        if (pwResult.Success is false)
            return Result.Fail<Aggregates.User>(pwResult.Error);

        var user = new Aggregates.User(Guid.NewGuid(), emailResult.Value, pwResult.Value, isVerified: false);

        user.AddVerificationCode();
        user.AddDomainEvent(new UserCreatedEvent(user.Id));

        return user;
    }
}
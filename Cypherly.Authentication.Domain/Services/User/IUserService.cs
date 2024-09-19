using Cypherly.Authentication.Domain.Entities;
using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IUserService
{
    Result<Aggregates.User> CreateUser(string email, string password);
    void GenerateVerificationCode(Aggregates.User user, VerificationCodeType codeType);
    RefreshToken GenerateRefreshToken(Aggregates.User user);
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

        //TODO: consider moving this to GenerateVerificationCode method and implement some generic email event
        user.AddVerificationCode(VerificationCodeType.EmailVerification);
        user.AddDomainEvent(new UserCreatedEvent(user.Id));

        return user;
    }

    public void GenerateVerificationCode(Aggregates.User user, VerificationCodeType codeType)
    {
        user.AddVerificationCode(codeType);
        user.AddDomainEvent(new VerificationCodeGeneratedEvent(user.Id, codeType));
    }

    public RefreshToken GenerateRefreshToken(Aggregates.User user)
    {
        user.AddRefreshToken();
        return user.GetActiveRefreshToken()!;
    }
}
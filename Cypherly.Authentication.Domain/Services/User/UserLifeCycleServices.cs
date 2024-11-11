using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;
using Cypherly.Authentication.Domain.ValueObjects;
using Cypherly.Domain.Common;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IUserLifeCycleServices
{
    Result<Aggregates.User> CreateUser(string email, string password);
    void SoftDelete(Aggregates.User user);
    void RevertSoftDelete(Aggregates.User user);
    bool IsUserDeleted(Aggregates.User user);
}

public class UserLifeCycleServices : IUserLifeCycleServices
{
    public Result<Aggregates.User> CreateUser(string email, string password)
    {
        var emailResult = Email.Create(email);
        if (emailResult.Success is false)
            return Result.Fail<Aggregates.User>(emailResult.Error);

        var pwResult = Password.Create(password);
        if (pwResult.Success is false)
            return Result.Fail<Aggregates.User>(pwResult.Error);

        var user = new Aggregates.User(Guid.NewGuid(), emailResult.Value!, pwResult.Value!, isVerified: false);

        user.AddVerificationCode(VerificationCodeType.EmailVerification);
        user.AddDomainEvent(new UserCreatedEvent(user.Id));

        return user;
    }

    public void SoftDelete(Aggregates.User user)
    {
        user.SetDelete();
        user.AddDomainEvent(new UserDeletedEvent(user.Id, user.Email.Address));
    }

    public void RevertSoftDelete(Aggregates.User user)
    {
        user.RevertDelete();
    }

    public bool IsUserDeleted(Aggregates.User user)
    {
        return user.DeletedAt.HasValue;
    }
}
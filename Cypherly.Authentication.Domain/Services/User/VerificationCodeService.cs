using Cypherly.Authentication.Domain.Enums;
using Cypherly.Authentication.Domain.Events.User;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IVerificationCodeService
{
    void GenerateVerificationCode(Aggregates.User user, VerificationCodeType codeType);
}

public class VerificationCodeService : IVerificationCodeService
{
    public void GenerateVerificationCode(Aggregates.User user, VerificationCodeType codeType)
    {
        user.AddVerificationCode(codeType);
        user.AddDomainEvent(new VerificationCodeGeneratedEvent(user.Id, codeType));
    }
}
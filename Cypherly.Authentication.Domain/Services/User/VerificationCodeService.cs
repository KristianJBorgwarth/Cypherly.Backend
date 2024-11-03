using Cypherly.Authentication.Domain.Enums;

namespace Cypherly.Authentication.Domain.Services.User;

public interface IVerificationCodeService
{
    void GenerateVerificationCode(Aggregates.User user, VerificationCodeType codeType);

}
public class VerificationCodeService : IVerificationCodeService
{
    public void GenerateVerificationCode(Aggregates.User user, VerificationCodeType codeType)
    {
        throw new NotImplementedException();
    }
}
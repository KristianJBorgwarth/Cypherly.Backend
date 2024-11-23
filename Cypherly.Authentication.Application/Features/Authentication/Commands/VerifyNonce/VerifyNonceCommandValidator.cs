using FluentValidation;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyNonce;

public class VerifyNonceCommandValidator : AbstractValidator<VerifyNonceCommand>
{
    public VerifyNonceCommandValidator()
    {

    }
}
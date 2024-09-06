using FluentValidation;

namespace MinimalEmail.API.Features.Requests;

public class SendEmailRequestValidator : AbstractValidator<SendEmailRequest>
{
    public SendEmailRequestValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.To)
            .NotNull().NotEmpty().WithMessage("Recipient email address is required.")
            .EmailAddress().WithMessage("Recipient email address is invalid.");

        RuleFor(x => x.Subject)
            .NotNull().NotEmpty().WithMessage("Email subject is required.")
            .MaximumLength(100).WithMessage("Email subject is too long.");

        RuleFor(x => x.Body)
            .NotNull().NotEmpty().WithMessage("Email body is required.");
    }
}
using System.Text.RegularExpressions;
using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.Login;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleLevelCascadeMode = CascadeMode.Stop;

        RuleFor(x => x.Email)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(LoginCommand.Email)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(LoginCommand.Email)).Message)
            .Must(email => email.Length <= 255).WithMessage(Errors.General.ValueTooLarge(nameof(LoginCommand.Email), 255).Message);

        RuleFor(x => x.Password)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(LoginCommand.Password)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(LoginCommand.Password)).Message)
            .Must(pw=> pw.Length <= 255).WithMessage(Errors.General.ValueTooLarge(nameof(LoginCommand.Password), 255).Message);

        RuleFor(x=> x.Base64DevicePublicKey)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(LoginCommand.Base64DevicePublicKey)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(LoginCommand.Base64DevicePublicKey)).Message)
            .Must(x=> x.Length <= 100).WithMessage(Errors.General.ValueTooLarge(nameof(LoginCommand.Base64DevicePublicKey), 100).Message);

        RuleFor(x=> x.DeviceType)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(LoginCommand.DeviceType)).Message);

        RuleFor(x=> x.DevicePlatform)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(LoginCommand.DevicePlatform)).Message);

        RuleFor(x => x.DeviceAppVersion)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(LoginCommand.DeviceAppVersion)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(LoginCommand.DeviceAppVersion)).Message)
            .Must(x => x.Length <= 6).WithMessage(Errors.General.ValueTooLarge(nameof(LoginCommand.DeviceAppVersion), 6).Message)
            .Must(BeValidDeviceAppVersion)
            .WithMessage(Errors.General.UnexpectedValue(nameof(LoginCommand.DeviceAppVersion)).Message);
    }

    private static bool BeValidDeviceAppVersion(string appVersion)
    {
        return Regex.IsMatch(appVersion, @"^\d+\.\d{1,3}$");
    }
}
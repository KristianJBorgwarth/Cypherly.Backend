using Cypherly.Domain.Common;
using FluentValidation;

namespace Cypherly.Authentication.Application.Features.User.Commands.Update.VerifyDevice;

public class VerifyDeviceCommandValidator : AbstractValidator<VerifyDeviceCommand>
{
    public VerifyDeviceCommandValidator()
    {
        RuleFor(x=> x.UserId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(VerifyDeviceCommand.UserId)).Message);

        RuleFor(x=> x.DeviceId)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(VerifyDeviceCommand.DeviceId)).Message);

        RuleFor(x=> x.DeviceVerificationCode)
            .NotNull().WithMessage(Errors.General.ValueIsRequired(nameof(VerifyDeviceCommand.DeviceVerificationCode)).Message)
            .NotEmpty().WithMessage(Errors.General.ValueIsEmpty(nameof(VerifyDeviceCommand.DeviceVerificationCode)).Message)
            .Must(x=> x.Length < 30).WithMessage(Errors.General.ValueTooLarge(nameof(VerifyDeviceCommand.DeviceVerificationCode), 30).Message);
    }
}
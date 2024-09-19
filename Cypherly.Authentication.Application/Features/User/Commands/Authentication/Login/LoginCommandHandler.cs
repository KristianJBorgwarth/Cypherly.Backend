using AutoMapper;
using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Services.Authentication;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;

namespace Cypherly.Authentication.Application.Features.User.Commands.Authentication.Login;

public class LoginCommandHandler(
    IUserRepository userRepository,
    IJwtService jwtService,
    ILogger<LoginCommandHandler> logger,
    IMapper mapper)
    : ICommandHandler<LoginCommand, LoginDto>
{
    public async Task<Result<LoginDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByEmailAsync(request.Email);
            if(user is null) return Result.Fail<LoginDto>(Errors.General.UnspecifiedError("Invalid Credentials"));

            var pwResult = user.Password.Verify(request.Password);
            if(!pwResult) return Result.Fail<LoginDto>(Errors.General.UnspecifiedError("Invalid Credentials"));

            if(user.IsVerified == false) return Result.Fail<LoginDto>(Errors.General.UnspecifiedError("User is not verified"));

            var token = jwtService.GenerateToken(user.Id, user.Email.Address, user.GetUserClaims());


            return Result.Ok(new LoginDto());

        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An exception occured while attempting to login with email {Email}", request.Email);
            return Result.Fail<LoginDto>(Errors.General.UnspecifiedError("An exception occured while attempting to login"));
        }
    }
}
using System.Text;
using Cypherly.Application.Abstractions;
using Cypherly.Authentication.Application.Contracts;
using Cypherly.Authentication.Application.Services.Authentication;
using Cypherly.Domain.Common;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;


namespace Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyNonce;

public class VerifyNonceCommandHandler(
    IUserRepository userRepository,
    INonceCacheService nonceCacheService,
    IJwtService jwtService,
    ILogger<VerifyNonceCommandHandler> logger)
    : ICommandHandler<VerifyNonceCommand, VerifyNonceDto>
{
    public async Task<Result<VerifyNonceDto>> Handle(VerifyNonceCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(request.UserId);

            if (user is null)
            {
                logger.LogWarning("User with ID: {ID} not found.", request.UserId);
                return Result.Fail<VerifyNonceDto>(Errors.General.NotFound(request.UserId));
            }

            var nonce = await nonceCacheService.GetNonceAsync(request.NonceId, cancellationToken);

            if (nonce is null)
            {
                logger.LogWarning("Nonce with ID: {ID} not found.", request.NonceId);
                return Result.Fail<VerifyNonceDto>(Errors.General.NotFound(request.NonceId));
            }

            if (nonce.UserId != user.Id || nonce.DeviceId != request.DeviceId)
            {
                logger.LogWarning("Nonce with ID: {ID} does not match user with ID: {UserId} and device with ID: {DeviceId}.", request.NonceId, user.Id, request.DeviceId);
                return Result.Fail<VerifyNonceDto>(Errors.General.Unauthorized());
            }

            var device = user.GetDevice(request.DeviceId);

            var isNonceValid = VerifyNonce(nonce.NonceValue, request.Nonce, device.PublicKey);

            if (isNonceValid)
            {
                var token = jwtService.GenerateToken(user.Id, user.Email.Address, user.GetUserClaims());
                device.AddRefreshToken();
                var refreshToken = device.GetActiveRefreshToken();

                var dto = VerifyNonceDto.Map(token, refreshToken!);

                return Result.Ok(dto);
            }
            else
            {
                return Result.Fail<VerifyNonceDto>(Errors.General.Unauthorized());
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    private static bool VerifyNonce(string nonce, string signature, string publickey)
    {
        var publicKeyParam = new Ed25519PublicKeyParameters(Convert.FromBase64String(publickey));
        var nonceBytes = Encoding.UTF8.GetBytes(nonce);
        var signatureBytes = Convert.FromBase64String(signature);

        var verifier = SignerUtilities.GetSigner("Ed25519");
        verifier.Init(false, publicKeyParam);
        verifier.BlockUpdate(nonceBytes, 0, nonceBytes.Length);
        return verifier.VerifySignature(signatureBytes);
    }
}
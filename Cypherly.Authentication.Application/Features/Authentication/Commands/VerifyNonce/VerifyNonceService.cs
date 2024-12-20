using System.Text;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Security;

namespace Cypherly.Authentication.Application.Features.Authentication.Commands.VerifyNonce;

public interface IVerifyNonceService
{
    public bool VerifyNonce(string nonce, string signature, string publicKey);
}
public sealed class VerifyNonceService : IVerifyNonceService
{
    public bool VerifyNonce(string nonce, string signature, string publicKey)
    {
        var publicKeyParam = new Ed25519PublicKeyParameters(Convert.FromBase64String(publicKey));
        var nonceBytes = Encoding.UTF8.GetBytes(nonce);
        var signatureBytes = Convert.FromBase64String(signature);

        var verifier = SignerUtilities.GetSigner("Ed25519");
        verifier.Init(false, publicKeyParam);
        verifier.BlockUpdate(nonceBytes, 0, nonceBytes.Length);
        return verifier.VerifySignature(signatureBytes);
    }
}
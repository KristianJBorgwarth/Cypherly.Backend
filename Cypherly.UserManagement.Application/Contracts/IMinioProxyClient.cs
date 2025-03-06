namespace Cypherly.UserManagement.Application.Contracts;

public interface IMinioProxyClient
{
    public Task<(byte[] image, string imageType)?> GetImageFromMinioAsync(string url, CancellationToken cancellationToken = default);
}
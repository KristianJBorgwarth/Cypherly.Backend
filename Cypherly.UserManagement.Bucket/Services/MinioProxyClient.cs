using Cypherly.UserManagement.Application.Contracts;
using Microsoft.Extensions.Logging;

namespace Cypherly.UserManagement.Bucket.Services;

public class MinioProxyClient(
    HttpClient httpClient,
    ILogger<MinioProxyClient> logger)
    : IMinioProxyClient
{
    public async Task<(byte[] image, string imageType)?> GetImageFromMinioAsync(string url, CancellationToken cancellationToken = default)
    {
        var response = await httpClient.GetAsync(url, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            logger.LogError("Error getting image from Minio. Status code: {StatusCode}", response.StatusCode);
            return null;
        }

        var image = await response.Content.ReadAsByteArrayAsync(cancellationToken);

        var imageType = response.Content.Headers.ContentType?.MediaType ?? DetectimageFormat(image);

        return (image, imageType);
    }

    /// <summary>
    /// Detects thje image format (JPEG or PNG) based on the magic numbers
    /// </summary>
    /// <param name="imageData"></param>
    /// <returns></returns>
    private static string DetectimageFormat(byte[] imageData)
    {
        if(imageData.Length < 8)
            return "image/jpeg";

        if (imageData[0] == 0xFF && imageData[1] == 0xD8 && imageData[2] == 0xFF)
            return "image/jpeg"; // JPEG magic bytes: FF D8 FF

        if (imageData[0] == 0x89 && imageData[1] == 0x50 && imageData[2] == 0x4E &&
            imageData[3] == 0x47 && imageData[4] == 0x0D && imageData[5] == 0x0A &&
            imageData[6] == 0x1A && imageData[7] == 0x0A)
            return "image/png"; // PNG magic bytes: 89 50 4E 47 0D 0A 1A 0A

        return "image/jpeg";
    }
}
using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Storage.Configuration;
using Cypherly.UserManagement.Storage.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Cypherly.UserManagement.Storage.Services;

public class ProfilePictureService(
    IAmazonS3 s3Client,
    IOptions<MinioSettings> minioSettings,
    IFileValidator fileValidator) : IProfilePictureService
{
    private readonly string _bucketName = minioSettings.Value.ProfilePictureBucket;

    /// <summary>
    /// Uploads a profile picture for a user, replacing any existing profile picture.
    /// </summary>
    /// <param name="file">The profile picture file to upload.</param>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <returns>A result containing the key of the uploaded profile picture.</returns>
    public async Task<Result<string>> UploadProfilePictureAsync(IFormFile file, Guid userId)
    {
        if (!fileValidator.IsValidImageFile(file, out var errorMessage))
            return Result.Fail<string>(Errors.General.UnexpectedValue(errorMessage));

        var keyName = $"profile-pictures/{userId}{Path.GetExtension(file.FileName)}";

        await DeleteExistingProfilePictures(userId);

        await using var stream = file.OpenReadStream();
        var putRequest = new PutObjectRequest
        {
            BucketName = _bucketName,
            Key = keyName,
            InputStream = stream,
            ContentType = file.ContentType,
        };

        var response = await s3Client.PutObjectAsync(putRequest);
        return response.HttpStatusCode == HttpStatusCode.OK
            ? Result.Ok(keyName)
            : Result.Fail<string>(
                Errors.General.UnspecifiedError($"{response.HttpStatusCode}: Failed to upload profile picture"));
    }

    /// <summary>
    /// Retrieves the presigned URL for a user's profile picture.
    /// </summary>
    /// <param name="profilePictureUrl">The unique identifier of the user's profile picture.</param>
    /// <returns>A result containing the presigned URL of the user's profile picture.</returns>
    public async Task<Result<string>> GetPresignedProfilePictureUrlAsync(string profilePictureUrl)
    {
        var getRequest = new GetPreSignedUrlRequest
        {
            BucketName = _bucketName,
            Key = profilePictureUrl,
            Expires = DateTime.UtcNow.AddMinutes(10)
        };

        var url = await s3Client.GetPreSignedURLAsync(getRequest);
        return url is not null
            ? Result.Ok(url)
            : Result.Fail<string>(Errors.General.UnspecifiedError("Failed to generate presigned URL."));
    }

    /// <summary>
    /// Deletes a specific profile picture by its key.
    /// </summary>
    /// <param name="keyName">The key of the profile picture to delete.</param>
    /// <returns>A result indicating success or failure of the deletion operation.</returns>
    public async Task<Result> DeleteProfilePictureAsync(string keyName)
    {
        var deleteRequest = new DeleteObjectRequest
        {
            BucketName = _bucketName,
            Key = keyName,
        };

        var response = await s3Client.DeleteObjectAsync(deleteRequest);
        return response.HttpStatusCode is HttpStatusCode.NoContent or HttpStatusCode.OK
            ? Result.Ok(true)
            : Result.Fail(
                Errors.General.UnspecifiedError($"{response.HttpStatusCode}: Failed to delete profile picture"));
    }

    /// <summary>
    /// Deletes any existing profile pictures for a user, identified by their user ID.
    /// Used internally in <see cref="UploadProfilePictureAsync"/>.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    private async Task DeleteExistingProfilePictures(Guid userId)
    {
        var listRequest = new ListObjectsV2Request()
        {
            BucketName = _bucketName,
            Prefix = $"profile-pictures/{userId}"
        };

        var listResponse = await s3Client.ListObjectsV2Async(listRequest);

        // Guard clause: no profile pictures to delete
        if (listResponse.S3Objects is null || listResponse.S3Objects.Count == 0) return;

        var keys = listResponse.S3Objects.Select(o => o.Key);

        foreach (var key in keys)
        {
            await DeleteProfilePictureAsync(key);
        }
    }
}
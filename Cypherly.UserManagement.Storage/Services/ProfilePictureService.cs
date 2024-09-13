using Amazon.S3;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Application.Contracts;
using Cypherly.UserManagement.Storage.Configuration;
using Cypherly.UserManagement.Storage.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Cypherly.UserManagement.Storage.Services;

public class ProfilePictureService(
    IAmazonS3 s3Client,
    IOptions<MinioSettings> settings,
    IFileValidator fileValidator)
    : IProfilePictureService
{
    public Task<Result<string>> UploadProfilePictureAsync(IFormFile file, Guid userId)
    {
        throw new NotImplementedException();
    }

    public Task<Result<string>> GetPresignedProfilePictureUrlAsync(string keyName)
    {
        throw new NotImplementedException();
    }

    public Task<Result> DeleteProfilePictureAsync(string keyName)
    {
        throw new NotImplementedException();
    }
}
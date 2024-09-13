using Cypherly.Domain.Common;
using Microsoft.AspNetCore.Http;

namespace Cypherly.UserManagement.Application.Contracts;

public interface IProfilePictureService
{
    Task<Result<string>> UploadProfilePictureAsync(IFormFile file, Guid userId);
    Task<Result<string>>GetPresignedProfilePictureUrlAsync(string keyName);
    Task<Result> DeleteProfilePictureAsync(string keyName);
}
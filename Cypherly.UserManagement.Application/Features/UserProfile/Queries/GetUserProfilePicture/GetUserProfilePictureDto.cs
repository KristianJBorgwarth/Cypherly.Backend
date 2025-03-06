namespace Cypherly.UserManagement.Application.Features.UserProfile.Queries.GetUserProfilePicture;

public sealed class GetUserProfilePictureDto
{
    public string Base64ProfilePicture { get; private set; }
    public string ContentType { get; private set; }

    private GetUserProfilePictureDto(string base64ProfilePicture, string contentType)
    {
        Base64ProfilePicture = base64ProfilePicture;
        ContentType = contentType;
    }

    public static GetUserProfilePictureDto MapFrom(byte[] imageBytes, string contentType)
    {
        var base64ProfilePicture = Convert.ToBase64String(imageBytes);
        return new GetUserProfilePictureDto(base64ProfilePicture, contentType);
    }
}
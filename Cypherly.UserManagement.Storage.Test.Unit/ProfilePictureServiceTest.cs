using System.Net;
using Amazon.S3;
using Amazon.S3.Model;
using Cypherly.Domain.Common;
using Cypherly.UserManagement.Storage.Configuration;
using Cypherly.UserManagement.Storage.Services;
using Cypherly.UserManagement.Storage.Test.Unit.Helpers;
using Cypherly.UserManagement.Storage.Validation;
using FakeItEasy;
using FluentAssertions;
using Microsoft.Extensions.Options;

namespace Cypherly.UserManagement.Storage.Test.Unit;

public class ProfilePictureServiceTests
{
    private readonly ProfilePictureService _profilePictureService;
    private readonly IAmazonS3 _fakeS3Client;
    private readonly IFileValidator _fakeFileValidator;

    public ProfilePictureServiceTests()
    {
        _fakeFileValidator = A.Fake<IFileValidator>();
        var fakeMinioSettings = A.Fake<IOptions<MinioSettings>>();
        _fakeS3Client = A.Fake<AmazonS3Client>();

        _profilePictureService = new(_fakeS3Client, fakeMinioSettings, _fakeFileValidator);
    }

    [Fact]
    public async void UploadProfilePictureAsync_Valid_Picture_Should_Upload_And_Return_Key()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileContent = new byte[] { 0x01, 0x02, 0x03 };
        var file = MockHelper.CreateFakeIFormFile("test.jpg", "image/jpeg", fileContent);
        string errormessage;
        A.CallTo(() => _fakeFileValidator.IsValidImageFile(file, out errormessage)).Returns(true);

        var putObjectResponse = new PutObjectResponse { HttpStatusCode = HttpStatusCode.OK };
        A.CallTo(() => _fakeS3Client.PutObjectAsync(A<PutObjectRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(putObjectResponse));

        // Act
        var result = await _profilePictureService.UploadProfilePictureAsync(file, userId);

        // Assert
        result.Success.Should().BeTrue();
        result.Value.Should().Be($"profile-pictures/{userId}.jpg");
    }

    [Fact]
    public async void UploadProfilePictureAsync_Invalid_Picture_Should_ReturnFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileContent = new byte[] { 0x01, 0x02, 0x03 };
        var file = MockHelper.CreateFakeIFormFile("test.jpg", "image/jpeg", fileContent);
        var errorMessage = "Invalid file format.";
        A.CallTo(() => _fakeFileValidator.IsValidImageFile(file, out errorMessage)).Returns(false);

        // Act
        var result = await _profilePictureService.UploadProfilePictureAsync(file, userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be(Errors.General.UnexpectedValue(errorMessage).Message);
    }

    [Fact]
    public async Task UploadProfilePictureAsync_S3_Failure_Should_ReturnFail()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var fileContent = new byte[] { 0x01, 0x02, 0x03 };
        var file = MockHelper.CreateFakeIFormFile("test.jpg", "image/jpeg", fileContent);
        string errorMessage;
        A.CallTo(() => _fakeFileValidator.IsValidImageFile(file, out errorMessage)).Returns(true);

        var putObjectResponse = new PutObjectResponse { HttpStatusCode = HttpStatusCode.InternalServerError };
        A.CallTo(() => _fakeS3Client.PutObjectAsync(A<PutObjectRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(putObjectResponse));

        // Act
        var result = await _profilePictureService.UploadProfilePictureAsync(file, userId);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be(Errors.General.UnspecifiedError($"{HttpStatusCode.InternalServerError}: Failed to upload profile picture").Message);
    }


    [Fact]
    public async void DeleteProfilePictureAsync_Valid_Delete_Should_Delete_And_Return_ResultOk()
    {
        // Arrange
        var keyName = "profile-pictures/test.jpg";
        var deleteObjectResponse = new DeleteObjectResponse { HttpStatusCode = HttpStatusCode.NoContent };
        A.CallTo(() => _fakeS3Client.DeleteObjectAsync(A<DeleteObjectRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(deleteObjectResponse));

        // Act
        var result = await _profilePictureService.DeleteProfilePictureAsync(keyName);

        // Assert
        result.Success.Should().BeTrue();
    }

    [Fact]
    public async void DeleteProfilePictureAsync_Invalid_Delete_Should_Return_ResultFail()
    {
        // Arrange
        var keyName = "profile-pictures/test.jpg";
        var deleteObjectResponse = new DeleteObjectResponse { HttpStatusCode = HttpStatusCode.InternalServerError };
        A.CallTo(() => _fakeS3Client.DeleteObjectAsync(A<DeleteObjectRequest>._, A<CancellationToken>._))
            .Returns(Task.FromResult(deleteObjectResponse));

        // Act
        var result = await _profilePictureService.DeleteProfilePictureAsync(keyName);

        // Assert
        result.Success.Should().BeFalse();
        result.Error.Message.Should().Be(Errors.General.UnspecifiedError($"{HttpStatusCode.InternalServerError}: Failed to delete profile picture").Message);
    }
}
using Cypherly.UserManagement.Storage.Validation;
using FakeItEasy;
using FluentAssertions;
using Microsoft.AspNetCore.Http;

namespace Cypherly.UserManagement.Storage.Test.Unit;

public class FileValidatorTests
{
    private readonly FileValidator _fileValidator = new();

    private static IFormFile CreateFakeFile(string fileName, string contentType, byte[] content)
    {
        var file = A.Fake<IFormFile>();
        A.CallTo(() => file.FileName).Returns(fileName);
        A.CallTo(() => file.ContentType).Returns(contentType);
        A.CallTo(() => file.Length).Returns(content.Length);
        A.CallTo(() => file.OpenReadStream()).Returns(new MemoryStream(content));
        return file;
    }

    [Fact]
    public void IsValidImageFile_Valid_Png_ReturnsTrue()
    {
        // Arrange
        var fileBytes = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }; // Valid PNG header
        var file = CreateFakeFile("test.png", "image/png", fileBytes);

        // Act
        var result = _fileValidator.IsValidImageFile(file, out var errorMessage);

        // Assert
        result.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }

    [Fact]
    public void IsValidImageFile_Valid_Jpg_ReturnsTrue()
    {
        // Arrange
        var fileBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xDB }; // Valid JPEG header
        var file = CreateFakeFile("test.jpg", "image/jpeg", fileBytes);

        // Act
        var result = _fileValidator.IsValidImageFile(file, out var errorMessage);

        // Assert
        result.Should().BeTrue();
        errorMessage.Should().BeEmpty();
    }

    [Fact]
    public void IsValidImageFile_Invalid_Extenstion_ReturnsFalse()
    {
        // Arrange
        var fileBytes = new byte[] { 0xFF, 0xD8, 0xFF, 0xDB }; // Valid JPEG header
        var file = CreateFakeFile("test.txt", "text/plain", fileBytes); // Invalid Extension

        // Act
        var result = _fileValidator.IsValidImageFile(file, out var errorMessage);

        // Assert
        result.Should().BeFalse();
        errorMessage.Should().Be("Invalid file type. Only JPG, JPEG and PNG files are allowed.");
    }

    [Fact]
    public void IsValidImageFile_Invalid_Size_ReturnsFalse()
    {
        // Arrange
        var fileBytes = new byte[5 * 1024 * 1024 + 1]; // Just over 5MB
        var file = CreateFakeFile("test.jpg", "image/jpeg", fileBytes);

        // Act
        var result = _fileValidator.IsValidImageFile(file, out var errorMessage);

        // Assert
        result.Should().BeFalse();
        errorMessage.Should().Be($"File size exceeds the maximum allowed size of {5 * 1024 * 1024} bytes");
    }

    [Fact]
    public void IsValidImageFile_Invalid_File_Signature_ReturnsFalse()
    {
        // Arrange
        var fileBytes = new byte[] { 0x00, 0x11, 0x22, 0x33 }; // Invalid JPEG or PNG header
        var file = CreateFakeFile("test.jpg", "image/jpeg", fileBytes);

        // Act
        var result = _fileValidator.IsValidImageFile(file, out var errorMessage);

        // Assert
        result.Should().BeFalse();
        errorMessage.Should().Be("Invalid file content. Only JPG, JPEG and PNG files are allowed.");
    }
}
using Microsoft.AspNetCore.Http;


namespace Cypherly.UserManagement.Bucket.Validation;

public interface IFileValidator
{
    bool IsValidImageFile(IFormFile file, out string errorMessage);
}

public class FileValidator : IFileValidator
{
    private const int MaxFileSize = 5 * 1024 * 1024; //5MB
    private static readonly string[] AllowedExtensions = [".jpg", ".jpeg", ".png"];

    public bool IsValidImageFile(IFormFile file, out string errorMessage)
    {
        if (!IsCorrectFileExtension(file))
        {
            errorMessage = "Invalid file type. Only JPG, JPEG and PNG files are allowed.";
            return false;
        }

        if (!IsCorrectSize(file))
        {
            errorMessage = $"File size exceeds the maximum allowed size of {MaxFileSize} bytes";
            return false;
        }

        if (!IsValidImageSignature(file))
        {
            errorMessage = "Invalid file content. Only JPG, JPEG and PNG files are allowed.";
            return false;
        }

        errorMessage = string.Empty;
        return true;
    }

    private static bool IsCorrectFileExtension(IFormFile file)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();
        return Array.Exists(AllowedExtensions, ext => ext == extension);
    }

    private static bool IsCorrectSize(IFormFile file)
    {
        return file.Length <= MaxFileSize;
    }

    private static bool IsValidImageSignature(IFormFile file)
    {
        using var reader = new BinaryReader(file.OpenReadStream());
        var bytes = reader.ReadBytes(8);
        file.OpenReadStream().Position = 0;

        // Check JPEG signature
        if (bytes.Length >= 2 && bytes[0] == 0xFF && bytes[1] == 0xD8)
        {
            return true;
        }

        // Check PNG signature
        if (bytes.Length >= 8 &&
            bytes[0] == 0x89 && bytes[1] == 0x50 &&
            bytes[2] == 0x4E && bytes[3] == 0x47 &&
            bytes[4] == 0x0D && bytes[5] == 0x0A &&
            bytes[6] == 0x1A && bytes[7] == 0x0A)
        {
            return true;
        }

        return false;
    }
}
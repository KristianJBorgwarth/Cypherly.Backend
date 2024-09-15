namespace Cypherly.UserManagement.Application.Test.Integration.Setup.Helpers;

public static class DirectoryHelper
{
    public static string GetProjectRootDirectory()
    {
        var directory = Directory.GetCurrentDirectory();
        while(directory is not null && !Directory.Exists(Path.Combine(directory, ".git")))
        {
            directory = Directory.GetParent(directory)?.FullName;
        }

        return directory ?? throw new Exception("Could not find project root directory.");
    }
}
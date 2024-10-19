using MyMobileFriends.Models;

namespace MyMobileFriends.Utilities;

public static class IoHelper
{
    public static bool MoveDirectory(string sourceDir, string destinationDir)
    {
        if (!Directory.Exists(sourceDir))
        {
            return false;
        }

        try
        {
            Directory.CreateDirectory(destinationDir);

            foreach (var directory in Directory.GetDirectories(sourceDir, "*", SearchOption.AllDirectories))
            {
                Directory.CreateDirectory(directory.Replace(sourceDir, destinationDir));
            }

            foreach (var file in Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories))
            {
                var destFile = file.Replace(sourceDir, destinationDir);
                if (File.Exists(destFile))
                {
                    File.Delete(destFile); // Delete if file already exists in new path
                }
                File.Move(file, destFile);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
        return true;
    }

    public static void EnsureUserDirectoryExists(User currentUser)
    {
        try
        {
            var userDirectory = Path.Combine(AppPaths.BaseFolderPath, "Users", currentUser.Username);
            if (!Directory.Exists(userDirectory))
            {
                Directory.CreateDirectory(userDirectory);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }

    }

    public static void DeleteUserDirectory(User currentUser)
    {
        try
        {
            var userDirectory = Path.Combine(AppPaths.BaseFolderPath, "Users", currentUser.Username);
            if (Directory.Exists(userDirectory))
            {
                Directory.Delete(userDirectory, true);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }

    }
}
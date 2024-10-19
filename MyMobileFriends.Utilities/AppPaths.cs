namespace MyMobileFriends.Utilities;

public static class AppPaths
{
    public static string BaseFolderPath { get; } = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
    public static string UsersFilePath { get; } = Path.Combine(BaseFolderPath, "users.json");

    public static string GetProfileFolderPath(string username)
    {
        Directory.CreateDirectory(Path.Combine(BaseFolderPath, "Users", username));
        return Path.Combine(BaseFolderPath, "Users", username);
    }
    public static string GetCharacterFilePath(string username, string characterName)
    {
        Directory.CreateDirectory(Path.Combine(GetProfileFolderPath(username), "Characters"));
        return Path.Combine(GetProfileFolderPath(username), "Characters", $"{characterName}.json");
    }
    public static string GetProfilePicturePath(string username, string extension)
    {
        Directory.CreateDirectory(Path.Combine(GetProfileFolderPath(username), "Profile Pictures"));
        return Path.Combine(GetProfileFolderPath(username), "Profile Pictures", $"{username}_Profile_Picture.{extension}");
    }
}

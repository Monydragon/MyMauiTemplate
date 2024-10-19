using MyMobileFriends.Extensions;
using MyMobileFriends.Models;
using MyMobileFriends.Services.Interfaces;
using MyMobileFriends.Utilities;
using Newtonsoft.Json;

namespace MyMobileFriends.Services;

public class UserServiceJson : IUserService
{
    public virtual User? CurrentUser { get; set; }
    public virtual List<User>? Users { get; set; } = new();

    public virtual async Task<List<User>> LoadUsersAsync()
    {
        try
        {
            if (!File.Exists(AppPaths.UsersFilePath))
            {
                return new List<User>();
            }

            var usersJson = await File.ReadAllTextAsync(AppPaths.UsersFilePath);
            return JsonConvert.DeserializeObject<List<User>>(usersJson) ?? new List<User>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task<User?> AuthenticateUser(string identifier, string password)
    {
        try
        {
            var users = await LoadUsersAsync();

            return users.FirstOrDefault(u =>
                (string.Equals(u.Username, identifier, StringComparison.CurrentCultureIgnoreCase) ||
                string.Equals(u.Email, identifier, StringComparison.CurrentCultureIgnoreCase)) &&
                u.Password.VerifyMatchingHash(password));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task SaveUserAsync(User user)
    {
        try
        {
            var users = await LoadUsersAsync();
            var existingUser = users.FirstOrDefault(u => u.Id == user.Id);

            if (existingUser == null)
            {
                users.Add(user);
            }
            else
            {
                existingUser.UpdateFrom(user);
            }

            await SaveUsersAsync(users);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task SaveUsersAsync(List<User> users)
    {
        try
        {
            var serializedUsers = JsonConvert.SerializeObject(users, Formatting.Indented);
            Directory.CreateDirectory(AppPaths.BaseFolderPath);
            await File.WriteAllTextAsync(AppPaths.UsersFilePath, serializedUsers);
            Users = users;
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task DeleteUserAsync(User user)
    {
        try
        {
            var users = await LoadUsersAsync();
            var existingUser = users.FirstOrDefault(u => u.Id == user.Id);

            if (existingUser != null)
            {
                users.Remove(existingUser);
                IoHelper.DeleteUserDirectory(existingUser);
                await SaveUsersAsync(users);
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task<bool> CheckUsernameExists(string username)
    {
        try
        {
            var users = await LoadUsersAsync();
            return users.Any(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task<bool> CheckEmailExists(string email)
    {
        try
        {
            var users = await LoadUsersAsync();
            if (string.IsNullOrWhiteSpace(email) || !users.Any())
                return false;

            return users.Any(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task<User?> GetUserById(Guid id)
    {
        try
        {
            var users = await LoadUsersAsync();
            return users.FirstOrDefault(u => u.Id == id);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task<User?> GetUserByUsername(string username)
    {
        try
        {
            var users = await LoadUsersAsync();
            return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task<User?> GetUserByEmail(string email)
    {
        try
        {
            var users = await LoadUsersAsync();
            return users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task<User?> GetUserByEmailOrUsername(string identifier)
    {
        try
        {
            var users = await LoadUsersAsync();
            return users.FirstOrDefault(u =>
                u.Username.Equals(identifier, StringComparison.InvariantCultureIgnoreCase) ||
                u.Email.Equals(identifier, StringComparison.InvariantCultureIgnoreCase));
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task SetUserVerificationCode(User user, VerificationCode verificationCode)
    {
        try
        {
            user.VerificationCode = verificationCode;
            await SaveUserAsync(user);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task<bool> VerifyUser(User user, string verificationCode)
    {
        try
        {
            return user.VerificationCode != null && await user.VerificationCode.IsValid(verificationCode);
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public virtual async Task MoveUserFiles(string oldUsername, string newUsername, User user)
    {
        try
        {
            var oldPath = AppPaths.GetProfileFolderPath(oldUsername);
            var newPath = AppPaths.GetProfileFolderPath(newUsername);

            if (Directory.Exists(oldPath))
            {
                Directory.CreateDirectory(newPath); // Ensure new directory exists
                IoHelper.MoveDirectory(oldPath, newPath);
                Directory.Delete(oldPath, true); // Delete old directory

                if (!string.IsNullOrWhiteSpace(user.ProfilePicturePath))
                {
                    if (user.ProfilePicturePath.StartsWith(oldPath))
                    {
                        user.ProfilePicturePath = user.ProfilePicturePath.Replace(oldPath, newPath);
                        await SaveUserAsync(user); // Update user data with new path
                    }
                }
            }
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }

    public async Task<User?> GetGuestUser()
    {
        var users = await LoadUsersAsync();
        return users.FirstOrDefault(u => u.Username.StartsWith("Guest", StringComparison.InvariantCultureIgnoreCase) && u.IsGuest);
    }
}

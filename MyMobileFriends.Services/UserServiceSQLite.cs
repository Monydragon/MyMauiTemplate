using Microsoft.EntityFrameworkCore;
using MyMobileFriends.Extensions;
using MyMobileFriends.Data.Data_Context;
using MyMobileFriends.Models;
using MyMobileFriends.Services.Interfaces;
using MyMobileFriends.Utilities;

namespace MyMobileFriends.Services;

public class UserServiceSQLite : IUserService
{
    protected readonly MyMauiTemplateAppContext _context;
    
    public User? CurrentUser { get; set; }
    public List<User>? Users { get; set; } = new();
    
    public UserServiceSQLite(MyMauiTemplateAppContext context)
    {
        _context = context;
    }
    
    public virtual async Task<List<User>> LoadUsersAsync()
    {
        try
        {
            if (_context.Users != null) Users = await _context.Users.ToListAsync();
            return Users ?? [];
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
            await LoadUsersAsync();

            if (Users == null || Users.Count == 0)
            {
                return null;
            }

            return Users.FirstOrDefault(u =>
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
            await LoadUsersAsync();
            
            var existingUser = Users?.FirstOrDefault(u => u.Id == user.Id);

            if (existingUser == null)
            {
                Users?.Add(user);
                if (_context.Users != null) await _context.Users.AddAsync(user);
            }
            else
            {
                existingUser.UpdateFrom(user);
            }
            
            await _context.SaveChangesAsync();
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
            _context.Users?.UpdateRange(users);
            await _context.SaveChangesAsync();
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
            await LoadUsersAsync();

            if (Users == null || Users.Count == 0)
            {
                return;
            }
            
            if(Users.Any(u => u.Id == user.Id))
            {
                Users.Remove(user);
                _context.Users?.Remove(user);
                await _context.SaveChangesAsync();
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
            await LoadUsersAsync();

            if (Users == null || Users.Count == 0)
            {
                return false;
            }
            
            return Users.Any(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
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
            await LoadUsersAsync();

            if (Users == null || Users.Count == 0 || string.IsNullOrWhiteSpace(email))
            {
                return false;
            }

            return Users.Any(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
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
            await LoadUsersAsync();

            if (Users == null || Users.Count == 0 || id == Guid.Empty)
            {
                return null;
            }
            
            return Users.FirstOrDefault(u => u.Id == id);
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
            await LoadUsersAsync();
            
            if (Users == null || Users.Count == 0)
            {
                return null;
            }
            
            return Users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.InvariantCultureIgnoreCase));
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
            await LoadUsersAsync();
            
            if (Users == null || Users.Count == 0)
            {
                return null;
            }
            return Users.FirstOrDefault(u => u.Email.Equals(email, StringComparison.InvariantCultureIgnoreCase));
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
            await LoadUsersAsync();
            
            if (Users == null || Users.Count == 0)
            {
                return null;
            }
            return Users.FirstOrDefault(u =>
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
        try
        {
            await LoadUsersAsync();
            
            if (Users == null || Users.Count == 0)
            {
                return null;
            }
            return Users.FirstOrDefault(u => u.Username.StartsWith("Guest", StringComparison.InvariantCultureIgnoreCase) && u.IsGuest);

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
            throw;
        }
    }
}
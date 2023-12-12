using MyMauiTemplate.Models;

namespace MyMauiTemplate.Services.Interfaces;

public interface IUserService
{
    User? CurrentUser { get; set; }
    List<User>? Users { get; set; }
    Task<List<User>> LoadUsersAsync();
    Task<User?> AuthenticateUser(string identifier, string password);
    Task SaveUserAsync(User user);
    Task DeleteUserAsync(User user);
    Task SaveUsersAsync(List<User> users);
    Task<bool> CheckUsernameExists(string username);
    Task<bool> CheckEmailExists(string email);
    Task<User?> GetUserById(Guid id);
    Task<User?> GetUserByUsername(string username);
    Task<User?> GetUserByEmail(string email);

    Task<User?> GetUserByEmailOrUsername(string identifier);

    Task SetUserVerificationCode(User user, VerificationCode verificationCode);

    Task<bool> VerifyUser(User user, string verificationCode);

    Task MoveUserFiles(string oldUsername, string newUsername, User user);

    Task<User?> GetGuestUser();
}
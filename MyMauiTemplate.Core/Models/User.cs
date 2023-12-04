namespace MyMauiTemplate.Core.Models;

public class User
{
    public Guid Id { get; set; }
    public string Username { get; set; }
    public string Password { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string ProfilePicturePath { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public string? TwoFactorSecret { get; set; }
    public List<string>? BackupCodes { get; set; }
    public bool Require2Fa { get; set; }
    public List<string>? TrustedDevices { get; set; }
    public bool StoreSavedPassword { get; set; }
    public VerificationCode? VerificationCode { get; set; }
    public bool IsGuest { get; set; }

    public User()
    {
        Id = Guid.NewGuid();
        Username = string.Empty;
        Password = string.Empty;
        Email = string.Empty;
        FirstName = string.Empty;
        LastName = string.Empty;
        ProfilePicturePath = string.Empty;
        TwoFactorEnabled = false;
        TwoFactorSecret = null;
        Require2Fa = false;
        StoreSavedPassword = false;
        IsGuest = false;
        BackupCodes = new List<string>();
        TrustedDevices = new List<string>();
        VerificationCode = null;
    }

    public User(string username, string password, string email, string firstName, string lastName)
    {
        Id = Guid.NewGuid();
        Username = username;
        Password = password;
        Email = email;
        FirstName = firstName;
        LastName = lastName;
        BackupCodes = new List<string>();
        TrustedDevices = new List<string>();
        VerificationCode = null;
        ProfilePicturePath = string.Empty;
    }

    public virtual void UpdateFrom(User user)
    {
        Username = user.Username;
        Password = user.Password;
        Email = user.Email;
        FirstName = user.FirstName;
        LastName = user.LastName;
        ProfilePicturePath = user.ProfilePicturePath;
        TwoFactorEnabled = user.TwoFactorEnabled;
        TwoFactorSecret = user.TwoFactorSecret;
        BackupCodes = user.BackupCodes;
        Require2Fa = user.Require2Fa;
        TrustedDevices = user.TrustedDevices;
        StoreSavedPassword = user.StoreSavedPassword;
        VerificationCode = user.VerificationCode;
        IsGuest = user.IsGuest;
    }
}
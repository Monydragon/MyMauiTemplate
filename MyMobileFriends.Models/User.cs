namespace MyMauiTemplate.Models;

public class User
{
    public virtual Guid Id { get; set; }
    public virtual string Username { get; set; }
    public virtual string Password { get; set; }
    public virtual string Email { get; set; }
    public virtual string FirstName { get; set; }
    public virtual string LastName { get; set; }
    public virtual string ProfilePicturePath { get; set; }
    public virtual bool TwoFactorEnabled { get; set; }
    public virtual string? TwoFactorSecret { get; set; }
    public virtual List<string>? BackupCodes { get; set; }
    public virtual bool Require2Fa { get; set; }
    public virtual List<string>? TrustedDevices { get; set; }
    public virtual bool StoreSavedPassword { get; set; }
    public virtual VerificationCode? VerificationCode { get; set; }
    public virtual bool IsGuest { get; set; }

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
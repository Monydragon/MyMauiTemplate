using MyMauiTemplate.Extensions;
using MyMauiTemplate.Models;
using OtpNet;

namespace MyMauiTemplate.Security;

public static class TwoFactorAuthenticator
{
    public static string GenerateSecretKey()
    {
        var key = KeyGeneration.GenerateRandomKey(20);
        return Base32Encoding.ToString(key);
    }

    public static (bool isValidated, bool backupCodeUsed) ValidateTotp(User user, string totp)
    {
        if (user.TwoFactorSecret == null)
        {
            return (false, false);
        }

        var totpEncrypted = totp.EncryptString();

        if (user.BackupCodes != null && user.BackupCodes.Contains(totpEncrypted))
        {
            user.BackupCodes.Remove(totpEncrypted);
            return (true, true);
        }

        var secretBytes = Base32Encoding.ToBytes(user.TwoFactorSecret.DecryptString());
        var totpGenerator = new Totp(secretBytes);
        return (totpGenerator.VerifyTotp(totp, out var timeStepMatched, new VerificationWindow(2, 2)), false);
    }
}
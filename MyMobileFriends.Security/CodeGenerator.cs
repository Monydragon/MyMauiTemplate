using MyMobileFriends.Extensions;
using MyMobileFriends.Models;

namespace MyMobileFriends.Security;

public static class CodeGenerator
{
    public static List<string> GenerateBackupCodes(int numberOfCodes = 5)
    {
        var codes = new List<string>();
        var random = new Random();
        for (var i = 0; i < numberOfCodes; i++)
        {
            var rndCode = random.Next(100000, 999999).ToString().EncryptString();
            if (!codes.Exists(r => r.DecryptString() == rndCode.DecryptString()))
            {
                codes.Add(rndCode); // Generate a 6-digit code
            }
            else
            {
                i--;
            }
        }
        return codes;
    }

    // Method to generate a verification code with an expiry date
    public static VerificationCode GenerateVerificationCode(int expiryDurationInMinutes)
    {
        var random = new Random();
        var code = random.Next(100000, 999999).ToString(); // Generate a 6-digit code
        var encryptedCode = code.EncryptString();
        var expirationTime = DateTime.UtcNow.AddMinutes(expiryDurationInMinutes);
        return new VerificationCode(encryptedCode, expirationTime);
    }

    // Method to generate a verification code with a specific expiry date
    public static VerificationCode GenerateVerificationCode(DateTime expirationTime)
    {
        var random = new Random();
        var code = random.Next(100000, 999999).ToString(); // Generate a 6-digit code
        var encryptedCode = code.EncryptString();
        return new VerificationCode(encryptedCode, expirationTime);
    }


}
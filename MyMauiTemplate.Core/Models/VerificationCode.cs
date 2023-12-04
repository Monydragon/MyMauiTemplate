using MyMauiTemplate.Core.Extensions;

namespace MyMauiTemplate.Core.Models;

public class VerificationCode
{
    public string Code { get; set; }
    public DateTime ExpirationTime { get; set; }

    public VerificationCode(string code, DateTime expirationTime)
    {
        Code = code;
        ExpirationTime = expirationTime;
    }

    // Method to check if the verification code is valid
    public async Task<bool> IsValid()
    {
        var isValid = await Task.FromResult(DateTime.UtcNow <= ExpirationTime);
        return isValid;
    }

    // Method to check if the verification code is valid
    public async Task<bool> IsValid(string code)
    {
        var isValid = await Task.FromResult(DateTime.UtcNow <= ExpirationTime && Code.DecryptString().Equals(code));
        return isValid;
    }
}
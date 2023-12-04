using System.Security.Cryptography;
using System.Text;
using MyMauiTemplate.Core.Constants;
using MyMauiTemplate.Core.Extensions;

namespace MyMauiTemplate.Core.Extensions;

public static class StringExtensions
{

    public static string EncryptString(this string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = AppConstants.EncryptionKey.ToByteArrayFromString() ?? Array.Empty<byte>();
        aes.IV = AppConstants.EncryptionIV.ToByteArrayFromString() ?? Array.Empty<byte>();

        var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
        using (var streamWriter = new StreamWriter(cryptoStream))
        {
            streamWriter.Write(plainText);
        }
        return Convert.ToBase64String(memoryStream.ToArray());
    }

    public static string DecryptString(this string cipherText)
    {
        var buffer = Convert.FromBase64String(cipherText);
        using var aes = Aes.Create();
        aes.Key = AppConstants.EncryptionKey.ToByteArrayFromString() ?? Array.Empty<byte>();
        aes.IV = AppConstants.EncryptionIV.ToByteArrayFromString() ?? Array.Empty<byte>();

        var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
        using var memoryStream = new MemoryStream(buffer);
        using var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
        using var streamReader = new StreamReader(cryptoStream);
        return streamReader.ReadToEnd();
    }

    public static string HashString(this string val)
    {
        using var algorithm = new Rfc2898DeriveBytes(
            val,
            AppConstants.EncryptionSaltSize,
            AppConstants.EncryptionIterations,
            HashAlgorithmName.SHA256);
        var key = Convert.ToBase64String(algorithm.GetBytes(AppConstants.EncryptionKeySize));
        var salt = Convert.ToBase64String(algorithm.Salt);

        return $"{AppConstants.EncryptionIterations}.{salt}.{key}";
    }


    public static bool VerifyMatchingHash(this string hashedVal, string hashToCheck)
    {
        var parts = hashedVal.Split('.', 3);

        if (parts.Length != 3)
        {
            throw new FormatException("Unexpected hash format. Should be formatted as '{iterations}.{salt}.{hash}'");
        }

        var iterations = Convert.ToInt32(parts[0]);
        var salt = Convert.FromBase64String(parts[1]);
        var key = Convert.FromBase64String(parts[2]);

        using var algorithm = new Rfc2898DeriveBytes(
            hashToCheck,
            salt,
            iterations,
            HashAlgorithmName.SHA256);
        var keyToCheck = algorithm.GetBytes(AppConstants.EncryptionKeySize);
        return keyToCheck.SequenceEqual(key);
    }

    public static byte[] ToByteArrayFromBase64(this string base64String)
    {
        return Convert.FromBase64String(base64String);
    }

    public static byte[]? ToByteArrayFromString(this string stringValue)
    {
        return string.IsNullOrWhiteSpace(stringValue) ? null : Encoding.UTF8.GetBytes(stringValue);
    }


    public static bool IsValidEmail(this string email)
    {
        try
        {
            var addr = new System.Net.Mail.MailAddress(email);
            return addr.Address == email;
        }
        catch
        {
            return false;
        }
    }
}

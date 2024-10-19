using MyMobileFriends.Utilities;
using SkiaSharp;
using SkiaSharp.QrCode.Image;

namespace MyMobileFriends.Security;

public static class TwoFaBarcodeGenerator
{

    public static Stream GenerateQrCodeStream(string secretKey, string username, string issuer)
    {
        var provisionUri = $"otpauth://totp/{issuer}:{username}?secret={secretKey}&issuer={issuer}";
        var qrCode = new QrCode(provisionUri, new Vector2Slim(256, 256), SKEncodedImageFormat.Png);

        var memoryStream = new MemoryStream();
        qrCode.GenerateImage(memoryStream);
        memoryStream.Position = 0;

        return memoryStream;
    }

    public static void TestGenerateQrCodeToFile(string secretKey, string username, string issuer)
    {
        var provisionUri = $"otpauth://totp/{issuer}:{username}?secret={secretKey}&issuer={issuer}";

        var qrCode = new QrCode(provisionUri, new Vector2Slim(256, 256), SKEncodedImageFormat.Png);
        var path = Path.Combine(AppPaths.GetProfileFolderPath(username), $"{username}_qrcode.png");
        using var fileStream = File.Create(path);
        qrCode.GenerateImage(fileStream);
        fileStream.Close();
    }
}
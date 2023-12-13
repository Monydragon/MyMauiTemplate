namespace MyMauiTemplate.Configuration.Constants;

public static class AppConstants
{
    public const string EncryptionKey = "DoNotUseThisKeyThisIsJustATempk";
    public const string EncryptionIV = "DoNotUseThisIVky";
    public const int EncryptionSaltSize = 16;
    public const int EncryptionKeySize = 32;
    public const int EncryptionIterations = 10000;
    public const string DefaultFromEmail = "email@test.com";
    public const string DefaultFromEmailUser = "Test";
    public const string PostmarkDevelopmentApiKey = "PostmarkDevelopmentApiKey";
    public const string PostmarkProductionApiKey = "PostmarkProductionApiKey";
    public const string DefaultTransactionalStream = "DefaultTransactionalStream";
    public const string DefaultEmailTag = "DefaultEmailTag";
    public const string DatabaseName = "test.db";
    public const string SqlLiteConnectionString = $"Data Source={DatabaseName}";
    public const int WindowMinimumWidth = 400;
    public const int WindowMinimumHeight = 650;
}
namespace MyMauiTemplate.Core.Models;

public class EmailTemplate
{
    public string Subject { get; set; }
    public string HtmlContent { get; set; }
    public string PlainTextContent { get; set; }

    // Constructor to create an email template with subject and contents
    public EmailTemplate(string subject, string plainTextContent, string htmlContent)
    {
        Subject = subject;
        PlainTextContent = plainTextContent;
        HtmlContent = htmlContent;
    }

    // You can add methods to this class to generate specific templates
    public static EmailTemplate CreateVerificationTemplate(string username, string verificationCode)
    {
        var subject = "Your Verification Code";

        // Replace with your actual HTML content
        var htmlContent = $@"
        <html>
            <head>
                <style>
                    .code-box {{
                        border: 1px solid #D3D3D3;
                        padding: 10px;
                        font-size: 16px;
                        letter-spacing: 3px;
                        font-weight: bold;
                        display: inline-block;
                        margin-top: 10px;
                        margin-bottom: 10px;
                    }}
                    .email-container {{
                        font-family: Arial, sans-serif;
                        color: #333;
                        padding: 20px;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <h1>My Character Builder</h1>
                    <p>Hello {username},</p>
                    <p>Thank you for using My Character Builder! Here is your verification code:</p>
                    <div class='code-box'>{verificationCode}</div>
                    <p>Please enter this code in the app to verify your account.</p>
                    <p>(Code Expires in 24 Hours)</p>
                    <p>If you did not request this email, please contact our support team.</p>
                </div>
            </body>
        </html>";

        var plainTextContent = $"Hello {username},\n\n" +
                               $"Thank you for using My Character Builder! Here is your verification code: {verificationCode}\n\n" +
                               $"Please enter this code in the app to verify your account.\n\n" +
                               $"(Code Expires in 24 Hours)\n\n" +
                               $"If you did not request this email, please contact our support team.";

        return new EmailTemplate(subject, plainTextContent, htmlContent);
    }

    public static EmailTemplate CreatePasswordResetTemplate(string username, string resetCode)
    {
        var subject = "Your Password Reset Code";

        // Replace with your actual HTML content
        var htmlContent = $@"
        <html>
            <head>
                <style>
                    .code-box {{
                        border: 1px solid #D3D3D3;
                        padding: 10px;
                        font-size: 16px;
                        letter-spacing: 3px;
                        font-weight: bold;
                        display: inline-block;
                        margin-top: 10px;
                        margin-bottom: 10px;
                    }}
                    .email-container {{
                        font-family: Arial, sans-serif;
                        color: #333;
                        padding: 20px;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <h1>My Character Builder</h1>
                    <p>Hello {username},</p>
                    <p>Thank you for using My Character Builder! Here is your password reset code:</p>
                    <div class='code-box'>{resetCode}</div>
                    <p>Please enter this code in the app to reset your password.</p>
                    <p>(Code Expires in 24 Hours)</p>
                    <p>If you did not request this email, please contact our support team.</p>
                </div>
            </body>
        </html>";

        var plainTextContent = $"Hello {username},\n\n" +
                               $"Thank you for using My Character Builder! Here is your password reset code: {resetCode}\n\n" +
                               $"Please enter this code in the app to reset your password.\n\n" +
                               $"(Code Expires in 24 Hours)\n\n" +
                               $"If you did not request this email, please contact our support team.";

        return new EmailTemplate(subject, plainTextContent, htmlContent);
    }

    public static EmailTemplate CreatePasswordResetSuccessTemplate(string username)
    {
        var subject = "Your Password Has Been Reset";

        // Replace with your actual HTML content
        var htmlContent = $@"
        <html>
            <head>
                <style>
                    .email-container {{
                        font-family: Arial, sans-serif;
                        color: #333;
                        padding: 20px;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <h1>My Character Builder</h1>
                    <p>Hello {username},</p>
                    <p>Your password has been successfully reset.</p>
                    <p>If you did not request this email, please contact our support team.</p>
                </div>
            </body>
        </html>";

        var plainTextContent = $"Hello {username},\n\n" +
                               $"Your password has been successfully reset.\n\n" +
                               $"If you did not request this email, please contact our support team.";

        return new EmailTemplate(subject, plainTextContent, htmlContent);
    }

    public static EmailTemplate CreateAccountCreatedTemplate(string username)
    {
        var subject = "Your Account Has Been Created";

        // Replace with your actual HTML content
        var htmlContent = $@"
        <html>
            <head>
                <style>
                    .email-container {{
                        font-family: Arial, sans-serif;
                        color: #333;
                        padding: 20px;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <h1>My Character Builder</h1>
                    <p>Hello {username},</p>
                    <p>Your account has been successfully created.</p>
                    <p>If you did not request this email, please contact our support team.</p>
                </div>
            </body>
        </html>";

        var plainTextContent = $"Hello {username},\n\n" +
                               $"Your account has been successfully created.\n\n" +
                               $"If you did not request this email, please contact our support team.";

        return new EmailTemplate(subject, plainTextContent, htmlContent);
    }

    public static EmailTemplate CreateAccountDeletedTemplate(string username)
    {
        var subject = "Your Account Has Been Deleted";

        // Replace with your actual HTML content
        var htmlContent = $@"
        <html>
            <head>
                <style>
                    .email-container {{
                        font-family: Arial, sans-serif;
                        color: #333;
                        padding: 20px;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <h1>My Character Builder</h1>
                    <p>Hello {username},</p>
                    <p>Your account has been successfully deleted.</p>
                    <p>If you did not request this email, please contact our support team.</p>
                </div>
            </body>
        </html>";

        var plainTextContent = $"Hello {username},\n\n" +
                               $"Your account has been successfully deleted.\n\n" +
                               $"If you did not request this email, please contact our support team.";

        return new EmailTemplate(subject, plainTextContent, htmlContent);
    }

    public static EmailTemplate TwoFactorAddedTemplate(string username, List<string> backupCodes)
    {
        var subject = "Your Two Factor Authentication Has Been Enabled";

        // Replace with your actual HTML content
        var htmlContent = $@"
        <html>
            <head>
                <style>
                    .code-box {{
                        border: 1px solid #D3D3D3;
                        padding: 10px;
                        font-size: 16px;
                        letter-spacing: 3px;
                        font-weight: bold;
                        display: inline-block;
                        margin-top: 10px;
                        margin-bottom: 10px;
                    }}
                    .email-container {{
                        font-family: Arial, sans-serif;
                        color: #333;
                        padding: 20px;
                        text-align: center;
                    }}
                </style>
            </head>
            <body>
                <div class='email-container'>
                    <h1>My Character Builder</h1>
                    <p>Hello {username},</p>
                    <p>Your two factor authentication has been successfully enabled.</p>
                    <p>Here are your backup codes:</p>
                    <div class='code-box'>{string.Join("<br>", backupCodes)}</div>
                    <p>Please store these codes in a safe place.</p>
                    <p>If you did not request this email, please contact our support team.</p>
                </div>
            </body>
        </html>";

        var plainTextContent = $"Hello {username},\n\n" +
                               $"Your two factor authentication has been successfully enabled.\n\n" +
                               $"Here are your backup codes:\n\n" +
                               $"{string.Join("\n", backupCodes)}\n\n" +
                               $"Please store these codes in a safe place.\n\n" +
                               $"If you did not request this email, please contact our support team.";

        return new EmailTemplate(subject, plainTextContent, htmlContent);
    }
}

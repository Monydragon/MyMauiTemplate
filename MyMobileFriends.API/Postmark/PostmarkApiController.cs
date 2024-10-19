
using MyMauiTemplate.Configuration.Constants;
using MyMauiTemplate.Models;
using PostmarkDotNet;
using PostmarkDotNet.Model;

namespace MyMauiTemplate.API.Postmark;

public class PostmarkApiController
{
    private static PostmarkApiController? _instance;

    private readonly PostmarkClient _client;

    public static PostmarkApiController Instance
    {
        get { return _instance ??= new PostmarkApiController(); }
    }

    private PostmarkApiController()
    {
        var apiKey = AppConstants.PostmarkDevelopmentApiKey;
        _client = new PostmarkClient(apiKey);
    }

    //A PostmarkResponse is returned from the Postmark API all parameters included in the response are available in the response object
    public async Task<PostmarkResponse> SendEmailAsync(string toEmail, string subject, string textBody, string htmlBody,
        string fromEmail = AppConstants.DefaultFromEmail, string tag = AppConstants.DefaultEmailTag,
        ICollection<PostmarkMessageAttachment>? attachments = null, HeaderCollection? headers = null,
        IDictionary<string, string>? metadata = null, string messageStream = AppConstants.DefaultTransactionalStream)
    {
        var message = new PostmarkMessage(fromEmail, toEmail, subject, textBody, htmlBody, headers, metadata, messageStream)
        {
            Tag = tag,
            Attachments = attachments
        };
        var sendResult = await _client.SendMessageAsync(message);

        Console.WriteLine(sendResult.Status == PostmarkStatus.Success
                ? $"Message sent to {toEmail} with ID {sendResult.MessageID}"
                : $"Message not sent to {toEmail} with ID {sendResult.MessageID} because {sendResult.Message}");

        return sendResult;
    }

    public async Task<PostmarkResponse> SendEmailAsync(string toEmail, EmailTemplate template,
        string fromEmail = AppConstants.DefaultFromEmail,
        string tag = AppConstants.DefaultEmailTag, ICollection<PostmarkMessageAttachment>? attachments = null,
        HeaderCollection? headers = null, IDictionary<string, string>? metadata = null,
        string messageStream = AppConstants.DefaultTransactionalStream)
    {
        var message = new PostmarkMessage(fromEmail, toEmail, template.Subject, template.PlainTextContent, template.HtmlContent, headers, metadata, messageStream)
        {
            Tag = tag,
            Attachments = attachments
        };
        var sendResult = await _client.SendMessageAsync(message);

        Console.WriteLine(sendResult.Status == PostmarkStatus.Success
                ? $"Message sent to {toEmail} with ID {sendResult.MessageID}"
                : $"Message not sent to {toEmail} with ID {sendResult.MessageID} because {sendResult.Message}");

        return sendResult;
    }
}
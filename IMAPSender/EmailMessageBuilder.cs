using MimeKit;

namespace IMAPSender;

/// <summary>
/// Builds email messages using the builder pattern with support for HTML content.
/// </summary>
public class EmailMessageBuilder
{
    private string? _from;
    private string? _fromName;
    private string? _to;
    private string? _toName;
    private string? _subject;
    private string? _body;
    private bool _isHtml;

    public EmailMessageBuilder From(string email, string? name = null)
    {
        _from = email;
        _fromName = name;
        return this;
    }

    public EmailMessageBuilder To(string email, string? name = null)
    {
        _to = email;
        _toName = name;
        return this;
    }

    public EmailMessageBuilder WithSubject(string subject)
    {
        _subject = subject;
        return this;
    }

    public EmailMessageBuilder WithBody(string body, bool isHtml = false)
    {
        _body = body;
        _isHtml = isHtml;
        return this;
    }

    public MimeMessage Build()
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(_from);
        ArgumentException.ThrowIfNullOrWhiteSpace(_to);
        ArgumentException.ThrowIfNullOrWhiteSpace(_subject);

        var message = new MimeMessage();
        message.From.Add(new MailboxAddress(_fromName ?? string.Empty, _from));
        message.To.Add(new MailboxAddress(_toName ?? string.Empty, _to));
        message.Subject = _subject;
        
        // Create body part based on content type
        // MimeKit uses TextFormat enum for plain text vs HTML
        var bodyPart = new TextPart(_isHtml ? MimeKit.Text.TextFormat.Html : MimeKit.Text.TextFormat.Plain)
        {
            Text = _body ?? string.Empty
        };
        
        message.Body = bodyPart;

        return message;
    }
}

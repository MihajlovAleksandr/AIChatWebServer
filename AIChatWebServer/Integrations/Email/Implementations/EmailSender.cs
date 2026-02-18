using AIChatWebServer.Integrations.Email.DTO;
using AIChatWebServer.Integrations.Email.Interfaces;
using AIChatWebServer.Utils.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace AIChatWebServer.Integrations.Email.Implementations
{
    public sealed class EmailSender : IEmailSender
    {
        private readonly string _smtpServer;
        private readonly int _port;
        private readonly string _senderEmail;
        private readonly string _password;

        private readonly IHtmlContentBuilder _htmlContentBuilder;
        private readonly ILogger<EmailSender> _logger;

        public EmailSender(
            IConfiguration configuration,
            IHtmlContentBuilder htmlContentBuilder,
            ILogger<EmailSender> logger)
        {
            _smtpServer = configuration["EmailSettings:SmtpServer"]
                ?? throw new ArgumentNullException(nameof(_smtpServer));

            if (!int.TryParse(configuration["EmailSettings:SmtpPort"], out int port))
                throw new ArgumentException("SMTP port is not valid.");

            _port = port;

            _senderEmail = configuration["EmailSettings:SenderEmail"]
                ?? throw new ArgumentNullException(nameof(_senderEmail));

            _password = configuration["EmailSettings:Password"]
                ?? throw new ArgumentNullException(nameof(_password));

            _htmlContentBuilder = htmlContentBuilder
                ?? throw new ArgumentNullException(nameof(htmlContentBuilder));

            _logger = logger
                ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task SendAsync(
            string email,
            EmailMessageRequest message,
            string[] imagePaths,
            CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                _logger.LogWarning("Recipient email is empty.");
                return;
            }

            if (message == null)
            {
                _logger.LogWarning("Email message is null.");
                return;
            }

            try
            {
                var mimeMessage = BuildMessage(
                    email,
                    message,
                    imagePaths);

                using var client = new SmtpClient();

                await client.ConnectAsync(
                    _smtpServer,
                    _port,
                    SecureSocketOptions.SslOnConnect,
                    cancellationToken);

                client.AuthenticationMechanisms.Remove("XOAUTH2");

                await client.AuthenticateAsync(
                    _senderEmail,
                    _password,
                    cancellationToken);

                await client.SendAsync(
                    mimeMessage,
                    cancellationToken);

                await client.DisconnectAsync(
                    true,
                    cancellationToken);

                _logger.LogInformation(
                    "Email sent to {Recipient}. Subject: {Subject}",
                    email,
                    message.Subject);
            }
            catch (OperationCanceledException)
            {
                _logger.LogWarning(
                    "Email sending cancelled for {Recipient}",
                    email);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error sending email to {Recipient}",
                    email);
            }
        }

        private MimeMessage BuildMessage(
            string recipient,
            EmailMessageRequest message,
            string[] imagePaths)
        {
            var mimeMessage = new MimeMessage();

            mimeMessage.From.Add(
                new MailboxAddress("AI Chat", _senderEmail));

            mimeMessage.To.Add(
                MailboxAddress.Parse(recipient));

            mimeMessage.Subject = message.Subject;

            var builder = new BodyBuilder();

            var htmlResult = _htmlContentBuilder.BuildHtml(
                message.Text,
                imagePaths);

            builder.HtmlBody = htmlResult.Html;
            builder.TextBody = message.Text;

            AttachInlineImages(
                builder,
                htmlResult);

            mimeMessage.Body = builder.ToMessageBody();

            return mimeMessage;
        }

        private void AttachInlineImages(
            BodyBuilder builder,
            HtmlContentResult htmlResult)
        {
            if (htmlResult.ImageMap == null ||
                htmlResult.ImageMap.Count == 0)
                return;

            foreach (var pair in htmlResult.ImageMap)
            {
                var path = pair.Key;
                var contentId = pair.Value;

                if (!File.Exists(path))
                {
                    _logger.LogWarning(
                        "Image not found: {Path}",
                        path);

                    continue;
                }

                var image = builder.LinkedResources.Add(path);

                image.ContentId = contentId;

                image.ContentDisposition =
                    new ContentDisposition(ContentDisposition.Inline);

                image.ContentType.MediaType = "image";
            }
        }
    }
}

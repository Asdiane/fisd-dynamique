using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Text;
using System.Text.RegularExpressions;
using Fisd.Application.Email.Models;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Diagnostics;
using Microsoft.Extensions.Options;

namespace Fisd.Application.Email
{
    public class GlobalEmailService : IGlobalEmailService
    {
        private readonly EmailOptions _options;
        private readonly FisdDbContext _dbContext;

        public GlobalEmailService(IOptions<EmailOptions> options, FisdDbContext dbContext)
        {
            _options = options.Value ?? throw new ArgumentNullException(nameof(options));
            _dbContext = dbContext;
        }

        public EmailOptions GetEmailOptions()
        {
            return _options;
        }

        public string MergeValues(string emailTemplate, Dictionary<string, string> values)
        {
            throw new NotImplementedException();
        }

        public async Task SendEmailAsync(Sender from, List<Message> messages, List<Models.Attachment>? attachments = null, double sendDelayInMinutes = 0, string templateType = "Generic", CancellationToken cancellationToken = default)
        {
            // Fail with a clear, actionable message instead of letting MailAddress throw a raw
            // "empty string 'address'" ArgumentException when the Email:Senders config section
            // is unset for this environment (same failure shape as the Fido2 config gap).
            if (string.IsNullOrWhiteSpace(_options.SmtpServer))
            {
                throw new InvalidOperationException("La configuration SMTP est vide (Email:SmtpServer). Aucun courriel ne peut être envoyé dans cet environnement.");
            }

            var fromAddress = !string.IsNullOrEmpty(from.FromAddress) ? from.FromAddress : from.Email;
            if (string.IsNullOrWhiteSpace(fromAddress))
            {
                throw new InvalidOperationException($"Aucune adresse d'expéditeur configurée pour '{from.Name}' (Email:Senders:...:Email ou FromAddress).");
            }

            using var smtpClient = new SmtpClient(_options.SmtpServer, _options.SmtpPort);

            smtpClient.EnableSsl = true;
            smtpClient.UseDefaultCredentials = false;
            smtpClient.Credentials = new NetworkCredential(from.Email, from.Password);

            var senderDomain = fromAddress.Contains('@') ? fromAddress[(fromAddress.IndexOf('@') + 1)..] : "localhost";

            foreach (var msg in messages)
            {
                // Logged automatically for every attempt - no caller needs to remember to do it.
                // Recorded before the send, then updated with the outcome, so a crash mid-send
                // still leaves a "failed" row behind instead of no trace at all. Logging failures
                // are swallowed on purpose (e.g. right after this ships, before its migration has
                // run in a given environment) - a missing history table must never block a real
                // email from going out.
                var history = new EmailSentHistoryEntity
                {
                    Subject = msg.Subject,
                    Body = msg.Body,
                    SentFrom = fromAddress,
                    SentTo = msg.To.Email,
                    EmailSent = false,
                    CreatedAt = DateTimeOffset.UtcNow
                };
                var historyLoggingAvailable = true;
                try
                {
                    _dbContext.EmailSentHistory.Add(history);
                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
                catch
                {
                    historyLoggingAvailable = false;
                }

                try
                {
                    using var mailMessage = new MailMessage();

                    mailMessage.From = new MailAddress(fromAddress, from.Name);
                    mailMessage.To.Add(new MailAddress(msg.To.Email, msg.To.Name));
                    var replyToAddress = !string.IsNullOrEmpty(msg.ReplyTo) ? msg.ReplyTo : fromAddress;
                    mailMessage.ReplyToList.Add(new MailAddress(replyToAddress, from.Name));
                    mailMessage.Subject = msg.Subject;
                    mailMessage.SubjectEncoding = Encoding.UTF8;
                    mailMessage.BodyEncoding = Encoding.UTF8;
                    mailMessage.HeadersEncoding = Encoding.UTF8;
                    mailMessage.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;
                    mailMessage.Priority = MailPriority.Normal;

                    var messageId = $"<{DateTime.UtcNow:yyyyMMddHHmmss}.{Guid.NewGuid()}@{senderDomain}>";
                    mailMessage.Headers.Add("Message-ID", messageId);

                    var plainText = HtmlToPlainText(msg.Body);
                    mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(plainText, Encoding.UTF8, MediaTypeNames.Text.Plain));
                    mailMessage.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(msg.Body, Encoding.UTF8, MediaTypeNames.Text.Html));

                    if (attachments?.Count > 0)
                    {
                        foreach (var attachment in attachments)
                        {
                            var attachmentBytes = Convert.FromBase64String(attachment.Content);
                            using var memoryStream = new MemoryStream(attachmentBytes);
                            var mailAttachment = new System.Net.Mail.Attachment(memoryStream, attachment.FileName, attachment.FileType);
                            mailMessage.Attachments.Add(mailAttachment);
                        }
                    }

                    await smtpClient.SendMailAsync(mailMessage, cancellationToken);
                    history.EmailSent = true;
                    if (historyLoggingAvailable)
                    {
                        try
                        {
                            await _dbContext.SaveChangesAsync(cancellationToken);
                        }
                        catch { /* history table unavailable - the email itself already went out */ }
                    }
                }
                catch (Exception ex)
                {
                    history.ErrorMessage = ex.Message;
                    if (historyLoggingAvailable)
                    {
                        try
                        {
                            await _dbContext.SaveChangesAsync(cancellationToken);
                        }
                        catch { /* history table unavailable */ }
                    }
                    throw;
                }
            }
        }

        private static string HtmlToPlainText(string html)
        {
            var withoutTags = Regex.Replace(html, "<(script|style)[^>]*>.*?</\\1>", string.Empty, RegexOptions.Singleline | RegexOptions.IgnoreCase);
            withoutTags = Regex.Replace(withoutTags, "<br\\s*/?>|</p>|</tr>", "\n", RegexOptions.IgnoreCase);
            withoutTags = Regex.Replace(withoutTags, "<[^>]+>", string.Empty);
            withoutTags = WebUtility.HtmlDecode(withoutTags);
            withoutTags = Regex.Replace(withoutTags, "[ \\t]+", " ");
            withoutTags = Regex.Replace(withoutTags, "\n{3,}", "\n\n");
            return withoutTags.Trim();
        }
    }
}

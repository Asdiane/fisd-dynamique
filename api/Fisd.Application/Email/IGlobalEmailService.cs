using Fisd.Application.Email.Models;

namespace Fisd.Application.Email
{
    public interface IGlobalEmailService
    {
        Task SendEmailAsync(Sender from, List<Message> messages, List<Models.Attachment>? attachments = null, double sendDelayInMinutes = 0, string templateType = "Generic", CancellationToken cancellationToken = default);
        string MergeValues(string emailTemplate, Dictionary<string, string> values);
        EmailOptions GetEmailOptions();
    }
}

using Fisd.Application.Email.Models;

namespace Fisd.Application.Email
{
    public class EmailOptions
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public Dictionary<string, Sender> Senders { get; set; } = [];
    }
}

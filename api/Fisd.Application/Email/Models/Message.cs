namespace Fisd.Application.Email.Models
{
    public class Message
    {
        public Recipient To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string? ReplyTo { get; set; }
    }
}

namespace Fisd.Application.Email.Models
{
    public class Sender
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string Address { get; set; }

        /// <summary>
        /// The address shown to recipients in the "From"/"Reply-To" headers and used to derive the
        /// Message-ID domain. Separate from <see cref="Email"/> because with a transactional email
        /// provider the SMTP AUTH login is often a technical account, not the brand's actual sending
        /// address. Falls back to <see cref="Email"/> when not set, so a plain SMTP config keeps working.
        /// </summary>
        public string FromAddress { get; set; }
    }
}

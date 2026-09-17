namespace Fisd.Application.Models.result.Testimonial
{
    // Admin-facing - both languages, for the edit form.
    public class TestimonialResultModel
    {
        public Guid Id { get; set; }
        public string AuthorName { get; set; }
        public string? ImageUrl { get; set; }
        public string? AuthorRoleFr { get; set; }
        public string? AuthorRoleEn { get; set; }
        public string ContentFr { get; set; }
        public string ContentEn { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Public-facing - resolved to the visitor's language.
    public class PublicTestimonialResultModel
    {
        public Guid Id { get; set; }
        public string AuthorName { get; set; }
        public string? ImageUrl { get; set; }
        public string? AuthorRole { get; set; }
        public string Content { get; set; }
        public int DisplayOrder { get; set; }
    }
}

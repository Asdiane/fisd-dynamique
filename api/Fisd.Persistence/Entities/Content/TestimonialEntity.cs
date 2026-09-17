using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Content
{
    public class TestimonialEntity
    {
        public Guid Id { get; set; }
        // A person's name isn't translated - only their role and the quote are.
        public string AuthorName { get; set; }
        public string? ImageUrl { get; set; }
        public string? AuthorRoleFr { get; set; }
        public string? AuthorRoleEn { get; set; }
        public string ContentFr { get; set; }
        public string ContentEn { get; set; }
        public bool IsVisible { get; set; }
        public ContentStatusEnum Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}

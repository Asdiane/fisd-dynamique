namespace Fisd.Application.Models.receive.Testimonial
{
    public class SaveTestimonialModel
    {
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
}

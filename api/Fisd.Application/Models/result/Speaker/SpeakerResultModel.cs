namespace Fisd.Application.Models.result.Speaker
{
    // Admin-facing - both languages, for the edit form.
    public class SpeakerResultModel
    {
        public Guid Id { get; set; }
        public Guid EditionId { get; set; }
        public string Name { get; set; }
        public string RoleFr { get; set; }
        public string RoleEn { get; set; }
        public string ImageUrl { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Public-facing - resolved to the visitor's language.
    public class PublicSpeakerResultModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Role { get; set; }
        public string ImageUrl { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}

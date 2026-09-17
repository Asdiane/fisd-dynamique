namespace Fisd.Application.Models.result.Participant
{
    // Admin-facing - both languages, for the edit form.
    public class ParticipantResultModel
    {
        public Guid Id { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }

    // Public-facing - resolved to the visitor's language.
    public class PublicParticipantResultModel
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}

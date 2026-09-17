using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Content
{
    public class ParticipantEntity
    {
        public Guid Id { get; set; }
        public string NameFr { get; set; }
        public string NameEn { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public bool IsVisible { get; set; }
        public ContentStatusEnum Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}

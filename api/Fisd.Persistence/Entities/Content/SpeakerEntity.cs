using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Content
{
    public class SpeakerEntity
    {
        public Guid Id { get; set; }
        public Guid EditionId { get; set; }
        // A person's name isn't translated - only their role/title and bio are.
        public string Name { get; set; }
        public string RoleFr { get; set; }
        public string RoleEn { get; set; }
        public string ImageUrl { get; set; }
        public string DescriptionFr { get; set; }
        public string DescriptionEn { get; set; }
        public bool IsVisible { get; set; }
        public ContentStatusEnum Status { get; set; }
        public int DisplayOrder { get; set; }

        public EditionEntity Edition { get; set; }
    }
}

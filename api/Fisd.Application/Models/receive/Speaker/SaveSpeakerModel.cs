namespace Fisd.Application.Models.receive.Speaker
{
    public class SaveSpeakerModel
    {
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
}

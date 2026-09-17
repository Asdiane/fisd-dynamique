namespace Fisd.Persistence.Entities.Content
{
    public class SouvenirPhotoEntity
    {
        public Guid Id { get; set; }
        public Guid SouvenirId { get; set; }
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }

        public SouvenirEntity Souvenir { get; set; }
    }
}

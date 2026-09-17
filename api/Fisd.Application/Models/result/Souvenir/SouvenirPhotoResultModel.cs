namespace Fisd.Application.Models.result.Souvenir
{
    public class SouvenirPhotoResultModel
    {
        public Guid Id { get; set; }
        public Guid SouvenirId { get; set; }
        public string ImageUrl { get; set; }
        public string? Caption { get; set; }
        public int DisplayOrder { get; set; }
    }
}

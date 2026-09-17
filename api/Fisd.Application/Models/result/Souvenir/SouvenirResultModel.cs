namespace Fisd.Application.Models.result.Souvenir
{
    public class SouvenirResultModel
    {
        public Guid Id { get; set; }
        public int Year { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public string Status { get; set; }
    }
}

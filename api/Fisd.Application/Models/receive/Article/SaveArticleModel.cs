namespace Fisd.Application.Models.receive.Article
{
    public class SaveArticleModel
    {
        public string Title { get; set; }
        public string? Excerpt { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public string Status { get; set; }
        public DateTimeOffset? PublishedAt { get; set; }
    }
}

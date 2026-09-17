using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Content
{
    public class ArticleEntity
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string? Slug { get; set; }
        public string? Excerpt { get; set; }
        public string Content { get; set; }
        public string? ImageUrl { get; set; }
        public ArticleStatusEnum Status { get; set; }
        public DateTimeOffset? PublishedAt { get; set; }
        public DateTimeOffset CreatedAt { get; set; }
        public DateTimeOffset UpdatedAt { get; set; }
    }
}

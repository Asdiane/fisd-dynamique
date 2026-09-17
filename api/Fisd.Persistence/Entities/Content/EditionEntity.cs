using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Content
{
    public class EditionEntity
    {
        public EditionEntity()
        {
            Days = new HashSet<ProgramDayEntity>();
            Speakers = new HashSet<SpeakerEntity>();
        }

        public Guid Id { get; set; }
        public int Year { get; set; }
        public string BadgeFr { get; set; }
        public string BadgeEn { get; set; }
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string TextFr { get; set; }
        public string TextEn { get; set; }
        public DateTimeOffset? StartDate { get; set; }
        public DateTimeOffset? EndDate { get; set; }
        public string? LocationLabelFr { get; set; }
        public string? LocationLabelEn { get; set; }
        public string? TicketingUrl { get; set; }
        public bool IsVisible { get; set; }
        public ContentStatusEnum Status { get; set; }
        // The site's "current" edition - only one row may have this set at a time (enforced in
        // EditionsService), and the homepage/public content loads whatever this points to
        // instead of always showing whichever year happens to be seeded first.
        public bool IsCurrent { get; set; }

        public ICollection<ProgramDayEntity> Days { get; set; }
        public ICollection<SpeakerEntity> Speakers { get; set; }
    }
}

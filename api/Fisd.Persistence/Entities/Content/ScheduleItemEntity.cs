namespace Fisd.Persistence.Entities.Content
{
    public class ScheduleItemEntity
    {
        public Guid Id { get; set; }
        public Guid ProgramDayId { get; set; }
        public string Time { get; set; }
        public string Title { get; set; }
        public string Tag { get; set; }
        public string Detail { get; set; }
        public string Location { get; set; }
        public int DisplayOrder { get; set; }

        public ProgramDayEntity ProgramDay { get; set; }
    }
}

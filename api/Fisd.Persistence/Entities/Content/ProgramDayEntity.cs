namespace Fisd.Persistence.Entities.Content
{
    public class ProgramDayEntity
    {
        public ProgramDayEntity()
        {
            Schedule = new HashSet<ScheduleItemEntity>();
        }

        public Guid Id { get; set; }
        public Guid EditionId { get; set; }
        public string Label { get; set; }
        public string DateLabel { get; set; }
        public int DisplayOrder { get; set; }

        public EditionEntity Edition { get; set; }
        public ICollection<ScheduleItemEntity> Schedule { get; set; }
    }
}

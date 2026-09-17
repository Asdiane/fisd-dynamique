namespace Fisd.Application.Models.result.Edition
{
    public class ProgramDayResultModel
    {
        public Guid Id { get; set; }
        public Guid EditionId { get; set; }
        public string Label { get; set; }
        public string DateLabel { get; set; }
        public int DisplayOrder { get; set; }
    }
}

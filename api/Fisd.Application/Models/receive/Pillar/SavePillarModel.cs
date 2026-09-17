namespace Fisd.Application.Models.receive.Pillar
{
    public class SavePillarModel
    {
        public string TitleFr { get; set; }
        public string TitleEn { get; set; }
        public string TextFr { get; set; }
        public string TextEn { get; set; }
        public string ImageUrl { get; set; }
        public string Icon { get; set; }
        public bool IsVisible { get; set; }
        public string Status { get; set; }
        public int DisplayOrder { get; set; }
    }
}

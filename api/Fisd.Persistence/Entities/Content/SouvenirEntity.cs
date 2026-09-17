using Fisd.Persistence.Enums;

namespace Fisd.Persistence.Entities.Content
{
    public class SouvenirEntity
    {
        public SouvenirEntity()
        {
            Photos = new HashSet<SouvenirPhotoEntity>();
        }

        public Guid Id { get; set; }
        public int Year { get; set; }
        public string Title { get; set; }
        public string? Description { get; set; }
        public SouvenirStatusEnum Status { get; set; }

        public ICollection<SouvenirPhotoEntity> Photos { get; set; }
    }
}

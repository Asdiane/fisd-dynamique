using Fisd.Application.Models.receive.Pillar;
using Fisd.Application.Services;

namespace Fisd.Tests
{
    public class PillarsServiceTests
    {
        private static SavePillarModel MakeModel(int displayOrder = 1, bool isVisible = true) => new()
        {
            TitleFr = "Un espace de concertations",
            TitleEn = "A space for dialogue",
            TextFr = "Un lieu de dialogue strategique.",
            TextEn = "A place for strategic dialogue.",
            ImageUrl = "https://cdn.fisd.ca/pillars/a.jpg",
            Icon = "01",
            IsVisible = isVisible,
            Status = "published",
            DisplayOrder = displayOrder
        };

        [Fact]
        public async Task GetVisibleAsync_ExcludesHidden_OrderedByDisplayOrder()
        {
            var db = TestDbContextFactory.Create();
            var service = new PillarsService(db, new FakeCurrentLanguageAccessor());

            var second = await service.CreateAsync(MakeModel(displayOrder: 2));
            var hidden = await service.CreateAsync(MakeModel(displayOrder: 1, isVisible: false));
            var first = await service.CreateAsync(MakeModel(displayOrder: 1));

            var visible = await service.GetVisibleAsync();

            Assert.Equal(2, visible.Count);
            Assert.DoesNotContain(visible, p => p.Id == hidden.Id);
            Assert.Equal(first.Id, visible[0].Id);
            Assert.Equal(second.Id, visible[1].Id);
        }

        [Fact]
        public async Task DeleteAsync_UnknownId_ReturnsFalse()
        {
            var db = TestDbContextFactory.Create();
            var service = new PillarsService(db, new FakeCurrentLanguageAccessor());

            Assert.False(await service.DeleteAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task UpdateAsync_ChangesPersistedFields()
        {
            var db = TestDbContextFactory.Create();
            var service = new PillarsService(db, new FakeCurrentLanguageAccessor());

            var created = await service.CreateAsync(MakeModel());
            var updateModel = MakeModel();
            updateModel.TitleFr = "Un cadre de plaidoyer";
            updateModel.Icon = "02";

            var updated = await service.UpdateAsync(created.Id, updateModel);

            Assert.NotNull(updated);
            Assert.Equal("Un cadre de plaidoyer", updated!.TitleFr);
            Assert.Equal("02", updated.Icon);
        }
    }
}

using Fisd.Application.Models.receive.Participant;
using Fisd.Application.Services;

namespace Fisd.Tests
{
    public class ParticipantsServiceTests
    {
        private static SaveParticipantModel MakeModel(int displayOrder = 1, bool isVisible = true) => new()
        {
            NameFr = "Associations",
            NameEn = "Associations",
            DescriptionFr = "Diasporas africaines, haitiennes et communautaires.",
            DescriptionEn = "African, Haitian and community diasporas.",
            IsVisible = isVisible,
            Status = "published",
            DisplayOrder = displayOrder
        };

        [Fact]
        public async Task GetVisibleAsync_ExcludesHidden_OrderedByDisplayOrder()
        {
            var db = TestDbContextFactory.Create();
            var service = new ParticipantsService(db, new FakeCurrentLanguageAccessor());

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
        public async Task GetAllAsync_IncludesHiddenRows()
        {
            var db = TestDbContextFactory.Create();
            var service = new ParticipantsService(db, new FakeCurrentLanguageAccessor());

            await service.CreateAsync(MakeModel(isVisible: false));

            Assert.Single(await service.GetAllAsync());
        }

        [Fact]
        public async Task DeleteAsync_RemovesRow()
        {
            var db = TestDbContextFactory.Create();
            var service = new ParticipantsService(db, new FakeCurrentLanguageAccessor());

            var created = await service.CreateAsync(MakeModel());
            Assert.True(await service.DeleteAsync(created.Id));
            Assert.Empty(await service.GetAllAsync());
        }
    }
}

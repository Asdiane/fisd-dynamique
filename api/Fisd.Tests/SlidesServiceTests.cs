using Fisd.Application.Models.receive.Slide;
using Fisd.Application.Services;

namespace Fisd.Tests
{
    public class SlidesServiceTests
    {
        private static SaveSlideModel MakeModel(int displayOrder = 1, bool isVisible = true) => new()
        {
            ImageUrl = "https://cdn.fisd.ca/slides/a.jpg",
            KickerFr = "Prochaine edition",
            KickerEn = "Next edition",
            TitleFr = "Rendez-vous a Montreal",
            TitleEn = "See you in Montreal",
            TextFr = "Trois jours d'immersion.",
            TextEn = "Three days of immersion.",
            PlaceFr = "Montreal, Canada",
            PlaceEn = "Montreal, Canada",
            IsVisible = isVisible,
            Status = "published",
            DisplayOrder = displayOrder
        };

        [Fact]
        public async Task GetVisibleAsync_ExcludesHiddenSlides_OrderedByDisplayOrder()
        {
            var db = TestDbContextFactory.Create();
            var service = new SlidesService(db, new FakeCurrentLanguageAccessor());

            var second = await service.CreateAsync(MakeModel(displayOrder: 2));
            var hidden = await service.CreateAsync(MakeModel(displayOrder: 1, isVisible: false));
            var first = await service.CreateAsync(MakeModel(displayOrder: 1));

            var visible = await service.GetVisibleAsync();

            Assert.Equal(2, visible.Count);
            Assert.DoesNotContain(visible, s => s.Id == hidden.Id);
            Assert.Equal(first.Id, visible[0].Id);
            Assert.Equal(second.Id, visible[1].Id);
        }

        [Fact]
        public async Task GetVisibleAsync_ResolvesRequestedLanguage()
        {
            var db = TestDbContextFactory.Create();
            var frService = new SlidesService(db, new FakeCurrentLanguageAccessor("fr"));
            await frService.CreateAsync(MakeModel());

            var enService = new SlidesService(db, new FakeCurrentLanguageAccessor("en"));
            var visibleEn = await enService.GetVisibleAsync();
            var visibleFr = await frService.GetVisibleAsync();

            Assert.Equal("Next edition", visibleEn[0].Kicker);
            Assert.Equal("Prochaine edition", visibleFr[0].Kicker);
        }

        [Fact]
        public async Task UpdateAsync_UnknownId_ReturnsNull()
        {
            var db = TestDbContextFactory.Create();
            var service = new SlidesService(db, new FakeCurrentLanguageAccessor());

            var result = await service.UpdateAsync(Guid.NewGuid(), MakeModel());

            Assert.Null(result);
        }

        [Fact]
        public async Task CreateThenUpdateThenDelete_RoundTrips()
        {
            var db = TestDbContextFactory.Create();
            var service = new SlidesService(db, new FakeCurrentLanguageAccessor());

            var created = await service.CreateAsync(MakeModel());
            var updated = await service.UpdateAsync(created.Id, MakeModel(displayOrder: 5));
            Assert.NotNull(updated);
            Assert.Equal(5, updated!.DisplayOrder);

            var deleted = await service.DeleteAsync(created.Id);
            Assert.True(deleted);
            Assert.Empty(await service.GetAllAsync());
        }
    }
}

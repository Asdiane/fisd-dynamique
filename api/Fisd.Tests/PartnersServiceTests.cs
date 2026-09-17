using Fisd.Application.Models.receive.Partner;
using Fisd.Application.Services;

namespace Fisd.Tests
{
    public class PartnersServiceTests
    {
        private static SavePartnerModel MakeModel(int displayOrder = 1, bool isVisible = true, string? label = null) => new()
        {
            Name = "Ville de Montreal",
            Label = label,
            LogoUrl = "https://cdn.fisd.ca/partners/montreal.png",
            IsVisible = isVisible,
            Status = "published",
            DisplayOrder = displayOrder
        };

        [Fact]
        public async Task GetVisibleAsync_ExcludesHidden_OrderedByDisplayOrder()
        {
            var db = TestDbContextFactory.Create();
            var service = new PartnersService(db);

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
        public async Task CreateAsync_OptionalLabel_DefaultsToNull()
        {
            var db = TestDbContextFactory.Create();
            var service = new PartnersService(db);

            var created = await service.CreateAsync(MakeModel());

            Assert.Null(created.Label);
        }

        [Fact]
        public async Task UpdateAsync_UnknownId_ReturnsNull()
        {
            var db = TestDbContextFactory.Create();
            var service = new PartnersService(db);

            Assert.Null(await service.UpdateAsync(Guid.NewGuid(), MakeModel()));
        }
    }
}

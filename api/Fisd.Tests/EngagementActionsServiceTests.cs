using Fisd.Application.Models.receive.EngagementAction;
using Fisd.Application.Services;

namespace Fisd.Tests
{
    public class EngagementActionsServiceTests
    {
        private static SaveEngagementActionModel MakeModel(int displayOrder = 1, bool isVisible = true) => new()
        {
            Anchor = "partenaire",
            TitleFr = "Devenir partenaire",
            TitleEn = "Become a partner",
            TextFr = "Associer votre organisation au FISD.",
            TextEn = "Associate your organization with FISD.",
            Icon = "PT",
            ImageUrl = "https://cdn.fisd.ca/engagement/partner.jpg",
            Link = "https://forms.example.com/partner",
            CtaFr = "Remplir le formulaire",
            CtaEn = "Fill out the form",
            DetailFr = "Pour les institutions et organisations.",
            DetailEn = "For institutions and organizations.",
            IsVisible = isVisible,
            Status = "published",
            DisplayOrder = displayOrder
        };

        [Fact]
        public async Task GetVisibleAsync_ExcludesHidden_OrderedByDisplayOrder()
        {
            var db = TestDbContextFactory.Create();
            var service = new EngagementActionsService(db, new FakeCurrentLanguageAccessor());

            var second = await service.CreateAsync(MakeModel(displayOrder: 2));
            var hidden = await service.CreateAsync(MakeModel(displayOrder: 1, isVisible: false));
            var first = await service.CreateAsync(MakeModel(displayOrder: 1));

            var visible = await service.GetVisibleAsync();

            Assert.Equal(2, visible.Count);
            Assert.DoesNotContain(visible, a => a.Id == hidden.Id);
            Assert.Equal(first.Id, visible[0].Id);
            Assert.Equal(second.Id, visible[1].Id);
        }

        [Fact]
        public async Task UpdateAsync_ChangesAnchorAndLink()
        {
            var db = TestDbContextFactory.Create();
            var service = new EngagementActionsService(db, new FakeCurrentLanguageAccessor());

            var created = await service.CreateAsync(MakeModel());
            var updateModel = MakeModel();
            updateModel.Anchor = "stand";
            updateModel.Link = "https://forms.example.com/stand";

            var updated = await service.UpdateAsync(created.Id, updateModel);

            Assert.NotNull(updated);
            Assert.Equal("stand", updated!.Anchor);
            Assert.Equal("https://forms.example.com/stand", updated.Link);
        }

        [Fact]
        public async Task DeleteAsync_RemovesRow()
        {
            var db = TestDbContextFactory.Create();
            var service = new EngagementActionsService(db, new FakeCurrentLanguageAccessor());

            var created = await service.CreateAsync(MakeModel());
            Assert.True(await service.DeleteAsync(created.Id));
            Assert.Empty(await service.GetAllAsync());
        }
    }
}

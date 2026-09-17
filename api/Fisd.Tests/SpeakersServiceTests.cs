using Fisd.Application.Models.receive.Speaker;
using Fisd.Application.Services;
using Fisd.Persistence.Entities.Content;

namespace Fisd.Tests
{
    public class SpeakersServiceTests
    {
        private static EditionEntity MakeEdition(int year) => new()
        {
            Id = Guid.NewGuid(),
            Year = year,
            BadgeFr = $"Edition {year}",
            BadgeEn = $"Edition {year}",
            TitleFr = $"Edition {year}",
            TitleEn = $"Edition {year}",
            TextFr = "Texte de presentation.",
            TextEn = "Presentation text.",
            IsVisible = true
        };

        private static SaveSpeakerModel MakeModel(Guid editionId, int displayOrder = 1, bool isVisible = true) => new()
        {
            EditionId = editionId,
            Name = "Benie Kouyate",
            RoleFr = "Presidente de Benie Foundation Inc.",
            RoleEn = "President of Benie Foundation Inc.",
            ImageUrl = "https://cdn.fisd.ca/speakers/benie.jpg",
            DescriptionFr = "Panéliste du FISD.",
            DescriptionEn = "FISD panelist.",
            IsVisible = isVisible,
            Status = "published",
            DisplayOrder = displayOrder
        };

        [Fact]
        public async Task GetVisibleByEditionAsync_OnlyReturnsSpeakersOfThatEdition()
        {
            var db = TestDbContextFactory.Create();
            var edition2025 = MakeEdition(2025);
            var edition2026 = MakeEdition(2026);
            db.Editions.AddRange(edition2025, edition2026);
            await db.SaveChangesAsync();

            var service = new SpeakersService(db, new FakeCurrentLanguageAccessor());
            var speaker2025 = await service.CreateAsync(MakeModel(edition2025.Id));
            await service.CreateAsync(MakeModel(edition2026.Id));

            var result = await service.GetVisibleByEditionAsync(edition2025.Id);

            var item = Assert.Single(result);
            Assert.Equal(speaker2025.Id, item.Id);
        }

        [Fact]
        public async Task GetVisibleByEditionAsync_HiddenSpeakerInSameEdition_IsExcluded()
        {
            var db = TestDbContextFactory.Create();
            var edition = MakeEdition(2026);
            db.Editions.Add(edition);
            await db.SaveChangesAsync();

            var service = new SpeakersService(db, new FakeCurrentLanguageAccessor());
            await service.CreateAsync(MakeModel(edition.Id, isVisible: false));
            var visibleOne = await service.CreateAsync(MakeModel(edition.Id));

            var result = await service.GetVisibleByEditionAsync(edition.Id);

            var item = Assert.Single(result);
            Assert.Equal(visibleOne.Id, item.Id);
        }

        [Fact]
        public async Task GetVisibleByEditionAsync_UnknownEdition_ReturnsEmpty()
        {
            var db = TestDbContextFactory.Create();
            var edition = MakeEdition(2026);
            db.Editions.Add(edition);
            await db.SaveChangesAsync();

            var service = new SpeakersService(db, new FakeCurrentLanguageAccessor());
            await service.CreateAsync(MakeModel(edition.Id));

            var result = await service.GetVisibleByEditionAsync(Guid.NewGuid());

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsSpeakersAcrossAllEditions()
        {
            var db = TestDbContextFactory.Create();
            var edition2025 = MakeEdition(2025);
            var edition2026 = MakeEdition(2026);
            db.Editions.AddRange(edition2025, edition2026);
            await db.SaveChangesAsync();

            var service = new SpeakersService(db, new FakeCurrentLanguageAccessor());
            await service.CreateAsync(MakeModel(edition2025.Id));
            await service.CreateAsync(MakeModel(edition2026.Id));

            Assert.Equal(2, (await service.GetAllAsync()).Count);
        }

        [Fact]
        public async Task UpdateAsync_CanMoveSpeakerToAnotherEdition()
        {
            var db = TestDbContextFactory.Create();
            var edition2025 = MakeEdition(2025);
            var edition2026 = MakeEdition(2026);
            db.Editions.AddRange(edition2025, edition2026);
            await db.SaveChangesAsync();

            var service = new SpeakersService(db, new FakeCurrentLanguageAccessor());
            var created = await service.CreateAsync(MakeModel(edition2025.Id));

            var updated = await service.UpdateAsync(created.Id, MakeModel(edition2026.Id));

            Assert.NotNull(updated);
            Assert.Equal(edition2026.Id, updated!.EditionId);
            Assert.Empty(await service.GetVisibleByEditionAsync(edition2025.Id));
            Assert.Single(await service.GetVisibleByEditionAsync(edition2026.Id));
        }
    }
}

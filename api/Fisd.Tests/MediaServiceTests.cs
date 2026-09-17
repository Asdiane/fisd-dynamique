using System.Text;
using Fisd.Application.Security;
using Fisd.Application.Services;
using Fisd.Application.Services.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Fisd.Tests
{
    public class MediaServiceTests : IDisposable
    {
        private readonly string _tempRoot = Path.Combine(Path.GetTempPath(), "fisd-media-tests-" + Guid.NewGuid());

        private MediaService BuildService(Fisd.Persistence.FisdDbContext db)
        {
            var options = Options.Create(new StorageOptions { MediaRootPath = _tempRoot, PublicBaseUrl = "https://cdn.fisd.ca" });
            return new MediaService(db, options, NullLogger<MediaService>.Instance);
        }

        private static Stream FakeImageStream() => new MemoryStream(Encoding.UTF8.GetBytes("fake-image-bytes"));

        [Fact]
        public async Task UploadAsync_UnknownCategory_Throws()
        {
            var db = TestDbContextFactory.Create();
            var service = BuildService(db);

            await Assert.ThrowsAsync<InvalidMediaException>(() =>
                service.UploadAsync("not-a-real-category", FakeImageStream(), "image/png", 100, "photo.png", null));
        }

        [Fact]
        public async Task UploadAsync_DisallowedContentType_Throws()
        {
            var db = TestDbContextFactory.Create();
            var service = BuildService(db);

            await Assert.ThrowsAsync<InvalidMediaException>(() =>
                service.UploadAsync("articles", FakeImageStream(), "application/pdf", 100, "doc.pdf", null));
        }

        [Fact]
        public async Task UploadAsync_TooLarge_Throws()
        {
            var db = TestDbContextFactory.Create();
            var service = BuildService(db);

            await Assert.ThrowsAsync<InvalidMediaException>(() =>
                service.UploadAsync("articles", FakeImageStream(), "image/png", 6 * 1024 * 1024, "photo.png", null));
        }

        [Theory]
        [InlineData("slides")]
        [InlineData("testimonials")]
        public async Task UploadAsync_ValidImage_WritesFileAndDbRow(string category)
        {
            var db = TestDbContextFactory.Create();
            var service = BuildService(db);

            var result = await service.UploadAsync(category, FakeImageStream(), "image/jpeg", 17, "hero.jpg", "Une photo du forum");

            Assert.Equal(category, result.Category);
            Assert.Equal("hero.jpg", result.OriginalFileName);
            Assert.Equal("Une photo du forum", result.AltText);
            Assert.StartsWith($"https://cdn.fisd.ca/media/{category}/", result.Url);
            Assert.Single(await service.GetAllAsync());
            Assert.True(File.Exists(Path.Combine(_tempRoot, category, Path.GetFileName(result.Url))));
        }

        [Fact]
        public async Task DeleteAsync_RemovesDbRowAndPhysicalFile()
        {
            var db = TestDbContextFactory.Create();
            var service = BuildService(db);

            var uploaded = await service.UploadAsync("partners", FakeImageStream(), "image/png", 17, "logo.png", null);
            var storedFileName = Path.GetFileName(uploaded.Url);
            var absolutePath = Path.Combine(_tempRoot, "partners", storedFileName);
            Assert.True(File.Exists(absolutePath));

            var deleted = await service.DeleteAsync(uploaded.Id);

            Assert.True(deleted);
            Assert.Empty(await service.GetAllAsync());
            Assert.False(File.Exists(absolutePath));
        }

        [Fact]
        public async Task DeleteAsync_UnknownId_ReturnsFalse()
        {
            var db = TestDbContextFactory.Create();
            var service = BuildService(db);

            Assert.False(await service.DeleteAsync(Guid.NewGuid()));
        }

        [Fact]
        public async Task GetAllAsync_OrdersMostRecentFirst()
        {
            var db = TestDbContextFactory.Create();
            var service = BuildService(db);

            var first = await service.UploadAsync("articles", FakeImageStream(), "image/png", 17, "a.png", null);
            await Task.Delay(5);
            var second = await service.UploadAsync("articles", FakeImageStream(), "image/png", 17, "b.png", null);

            var all = await service.GetAllAsync();

            Assert.Equal(second.Id, all[0].Id);
            Assert.Equal(first.Id, all[1].Id);
        }

        public void Dispose()
        {
            try
            {
                if (Directory.Exists(_tempRoot)) Directory.Delete(_tempRoot, recursive: true);
            }
            catch
            {
                // Best-effort cleanup only - a leftover temp dir doesn't affect other tests.
            }
        }
    }
}

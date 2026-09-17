using Fisd.Application.Models.result.Media;
using Fisd.Application.Security;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Media;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Fisd.Application.Services
{
    public class MediaService : IMediaService
    {
        private const long MaxImageBytes = 5 * 1024 * 1024;

        private static readonly Dictionary<string, string> AllowedImageTypes = new()
        {
            ["image/jpeg"] = "jpg",
            ["image/png"] = "png",
            ["image/webp"] = "webp"
        };

        // Every folder an image can be filed under - "category" from the route is checked against
        // this set before touching the filesystem, so it can never be used to write outside the
        // media root.
        private static readonly HashSet<string> AllowedCategories = new(StringComparer.OrdinalIgnoreCase)
        {
            "articles", "slides", "pillars", "speakers", "partners", "engagement-actions", "testimonials"
        };

        private readonly FisdDbContext _dbContext;
        private readonly StorageOptions _storageOptions;
        private readonly ILogger<MediaService> _logger;

        public MediaService(FisdDbContext dbContext, IOptions<StorageOptions> storageOptions, ILogger<MediaService> logger)
        {
            _dbContext = dbContext;
            _storageOptions = storageOptions.Value;
            _logger = logger;
        }

        public async Task<List<MediaFileResultModel>> GetAllAsync()
        {
            var files = await _dbContext.MediaFiles.OrderByDescending(m => m.CreatedAt).ToListAsync();
            return files.Select(ToResultModel).ToList();
        }

        public async Task<MediaFileResultModel> UploadAsync(string category, Stream content, string contentType, long lengthBytes, string originalFileName, string? altText)
        {
            if (!AllowedCategories.Contains(category))
            {
                throw new InvalidMediaException("Catégorie d'image inconnue.");
            }

            if (!AllowedImageTypes.TryGetValue(contentType, out var extension))
            {
                throw new InvalidMediaException("Type de fichier non autorisé.");
            }

            if (lengthBytes > MaxImageBytes)
            {
                throw new InvalidMediaException("L'image dépasse la taille maximale autorisée (5 Mo).");
            }

            var categoryFolder = category.ToLowerInvariant();
            var folder = Path.Combine(_storageOptions.MediaRootPath, categoryFolder);
            Directory.CreateDirectory(folder);

            var storedFileName = $"{Guid.NewGuid()}.{extension}";
            var relativePath = Path.Combine(categoryFolder, storedFileName);
            var fullPath = Path.Combine(_storageOptions.MediaRootPath, relativePath);

            await using (var fileStream = File.Create(fullPath))
            {
                await content.CopyToAsync(fileStream);
            }

            var entity = new MediaFileEntity
            {
                Id = Guid.NewGuid(),
                Category = categoryFolder,
                FileName = storedFileName,
                OriginalFileName = Path.GetFileName(originalFileName),
                ContentType = contentType,
                FileSizeBytes = lengthBytes,
                StoragePath = relativePath,
                PublicUrl = $"{_storageOptions.PublicBaseUrl.TrimEnd('/')}/media/{categoryFolder}/{storedFileName}",
                AltText = altText,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _dbContext.MediaFiles.Add(entity);
            await _dbContext.SaveChangesAsync();

            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.MediaFiles.FirstOrDefaultAsync(m => m.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.MediaFiles.Remove(entity);
            await _dbContext.SaveChangesAsync();

            var absolutePath = Path.Combine(_storageOptions.MediaRootPath, entity.StoragePath);
            try
            {
                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                }
            }
            catch (Exception ex)
            {
                // The DB row is the source of truth for the media library - a locked/missing file
                // on disk shouldn't surface as a failure to the caller.
                _logger.LogWarning(ex, "Failed to delete physical media file {AbsolutePath} for MediaFile {MediaFileId}", absolutePath, entity.Id);
            }

            return true;
        }

        private static MediaFileResultModel ToResultModel(MediaFileEntity entity) => new()
        {
            Id = entity.Id,
            Category = entity.Category,
            OriginalFileName = entity.OriginalFileName,
            ContentType = entity.ContentType,
            FileSizeBytes = entity.FileSizeBytes,
            Url = entity.PublicUrl,
            AltText = entity.AltText,
            CreatedAt = entity.CreatedAt
        };
    }
}

using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;
using Fisd.Application.Security;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Diagnostics;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Fisd.Application.Services
{
    public class TicketsService : ITicketsService
    {
        private const long MaxAttachmentBytes = 10 * 1024 * 1024;

        private static readonly Dictionary<string, string> AllowedAttachmentTypes = new()
        {
            ["image/jpeg"] = "jpg",
            ["image/png"] = "png",
            ["image/webp"] = "webp",
            ["application/pdf"] = "pdf",
            ["text/plain"] = "txt"
        };

        private readonly FisdDbContext _dbContext;
        private readonly StorageOptions _storageOptions;

        public TicketsService(FisdDbContext dbContext, IOptions<StorageOptions> storageOptions)
        {
            _dbContext = dbContext;
            _storageOptions = storageOptions.Value;
        }

        public async Task<List<TicketResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Tickets.OrderByDescending(t => t.CreatedAt).ToListAsync();
            return await ToResultModelsAsync(entities);
        }

        public async Task<List<TicketResultModel>> GetMineAsync(Guid adminUserId)
        {
            var entities = await _dbContext.Tickets
                .Where(t => t.CreatedByAdminUserId == adminUserId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
            return await ToResultModelsAsync(entities);
        }

        public async Task<TicketResultModel> CreateAsync(Guid adminUserId, string adminEmail, SaveTicketModel model)
        {
            var entity = new TicketEntity
            {
                Id = Guid.NewGuid(),
                Category = ParseCategory(model.Category),
                Title = model.Title,
                Description = model.Description,
                Status = TicketStatusEnum.Open,
                CreatedByAdminUserId = adminUserId,
                CreatedByEmail = adminEmail,
                CreatedAt = DateTimeOffset.UtcNow
            };

            _dbContext.Tickets.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<TicketResultModel?> UpdateStatusAsync(Guid id, UpdateTicketStatusModel model)
        {
            var entity = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
            {
                return null;
            }

            var status = ParseStatus(model.Status);
            entity.Status = status;
            entity.ResolutionNote = model.ResolutionNote;
            entity.ResolvedAt = status is TicketStatusEnum.Resolved or TicketStatusEnum.Rejected
                ? entity.ResolvedAt ?? DateTimeOffset.UtcNow
                : null;

            await _dbContext.SaveChangesAsync();
            var result = ToResultModel(entity);
            result.Attachments = await GetAttachmentsAsync(entity.Id);
            return result;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Tickets.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        // Only the ticket's own author or a PlatformAdmin can attach a file to it - reuses the
        // same allowed-type/size validation shape as MediaService, but tickets accept a broader
        // set of file kinds (screenshots, PDFs, logs) than the content-image library does.
        public async Task<TicketAttachmentResultModel?> UploadAttachmentAsync(Guid ticketId, Guid actingAdminId, bool isPlatformAdmin, Stream content, string contentType, long lengthBytes, string originalFileName)
        {
            var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null || (!isPlatformAdmin && ticket.CreatedByAdminUserId != actingAdminId))
            {
                return null;
            }

            if (!AllowedAttachmentTypes.TryGetValue(contentType, out var extension))
            {
                throw new InvalidMediaException("Type de fichier non autorisé (image, PDF ou texte seulement).");
            }
            if (lengthBytes > MaxAttachmentBytes)
            {
                throw new InvalidMediaException("Le fichier dépasse la taille maximale autorisée (10 Mo).");
            }

            var folder = Path.Combine(_storageOptions.MediaRootPath, "tickets", ticketId.ToString());
            Directory.CreateDirectory(folder);

            var storedFileName = $"{Guid.NewGuid()}.{extension}";
            var relativePath = Path.Combine("tickets", ticketId.ToString(), storedFileName);
            var fullPath = Path.Combine(_storageOptions.MediaRootPath, relativePath);

            await using (var fileStream = File.Create(fullPath))
            {
                await content.CopyToAsync(fileStream);
            }

            var attachment = new TicketAttachmentEntity
            {
                Id = Guid.NewGuid(),
                TicketId = ticketId,
                FileName = storedFileName,
                OriginalFileName = Path.GetFileName(originalFileName),
                ContentType = contentType,
                FileSizeBytes = lengthBytes,
                StoragePath = relativePath,
                PublicUrl = $"{_storageOptions.PublicBaseUrl.TrimEnd('/')}/media/tickets/{ticketId}/{storedFileName}",
                CreatedAt = DateTimeOffset.UtcNow
            };

            _dbContext.TicketAttachments.Add(attachment);
            await _dbContext.SaveChangesAsync();

            return ToAttachmentResultModel(attachment);
        }

        public async Task<bool> DeleteAttachmentAsync(Guid ticketId, Guid attachmentId, Guid actingAdminId, bool isPlatformAdmin)
        {
            var ticket = await _dbContext.Tickets.FirstOrDefaultAsync(t => t.Id == ticketId);
            if (ticket == null || (!isPlatformAdmin && ticket.CreatedByAdminUserId != actingAdminId))
            {
                return false;
            }

            var attachment = await _dbContext.TicketAttachments.FirstOrDefaultAsync(a => a.Id == attachmentId && a.TicketId == ticketId);
            if (attachment == null)
            {
                return false;
            }

            _dbContext.TicketAttachments.Remove(attachment);
            await _dbContext.SaveChangesAsync();

            var absolutePath = Path.Combine(_storageOptions.MediaRootPath, attachment.StoragePath);
            try
            {
                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                }
            }
            catch
            {
                // The DB row is the source of truth - a locked/missing file on disk shouldn't fail the request.
            }

            return true;
        }

        private async Task<List<TicketResultModel>> ToResultModelsAsync(List<TicketEntity> entities)
        {
            var ticketIds = entities.Select(t => t.Id).ToList();
            var attachmentsByTicket = (await _dbContext.TicketAttachments
                    .Where(a => ticketIds.Contains(a.TicketId))
                    .OrderBy(a => a.CreatedAt)
                    .ToListAsync())
                .GroupBy(a => a.TicketId)
                .ToDictionary(g => g.Key, g => g.Select(ToAttachmentResultModel).ToList());

            return entities.Select(e =>
            {
                var model = ToResultModel(e);
                model.Attachments = attachmentsByTicket.TryGetValue(e.Id, out var list) ? list : [];
                return model;
            }).ToList();
        }

        private async Task<List<TicketAttachmentResultModel>> GetAttachmentsAsync(Guid ticketId) =>
            await _dbContext.TicketAttachments
                .Where(a => a.TicketId == ticketId)
                .OrderBy(a => a.CreatedAt)
                .Select(a => new TicketAttachmentResultModel
                {
                    Id = a.Id,
                    OriginalFileName = a.OriginalFileName,
                    ContentType = a.ContentType,
                    FileSizeBytes = a.FileSizeBytes,
                    Url = a.PublicUrl,
                    CreatedAt = a.CreatedAt
                })
                .ToListAsync();

        private static TicketCategoryEnum ParseCategory(string category) => category switch
        {
            "bug" => TicketCategoryEnum.Bug,
            "improvement" => TicketCategoryEnum.Improvement,
            _ => TicketCategoryEnum.Other
        };

        private static string ToCategoryString(TicketCategoryEnum category) => category switch
        {
            TicketCategoryEnum.Bug => "bug",
            TicketCategoryEnum.Improvement => "improvement",
            _ => "other"
        };

        private static TicketStatusEnum ParseStatus(string status) => status switch
        {
            "in_progress" => TicketStatusEnum.InProgress,
            "resolved" => TicketStatusEnum.Resolved,
            "rejected" => TicketStatusEnum.Rejected,
            _ => TicketStatusEnum.Open
        };

        private static string ToStatusString(TicketStatusEnum status) => status switch
        {
            TicketStatusEnum.InProgress => "in_progress",
            TicketStatusEnum.Resolved => "resolved",
            TicketStatusEnum.Rejected => "rejected",
            _ => "open"
        };

        private static TicketResultModel ToResultModel(TicketEntity entity) => new()
        {
            Id = entity.Id,
            Category = ToCategoryString(entity.Category),
            Title = entity.Title,
            Description = entity.Description,
            Status = ToStatusString(entity.Status),
            CreatedByEmail = entity.CreatedByEmail,
            CreatedAt = entity.CreatedAt,
            ResolutionNote = entity.ResolutionNote,
            ResolvedAt = entity.ResolvedAt
        };

        private static TicketAttachmentResultModel ToAttachmentResultModel(TicketAttachmentEntity entity) => new()
        {
            Id = entity.Id,
            OriginalFileName = entity.OriginalFileName,
            ContentType = entity.ContentType,
            FileSizeBytes = entity.FileSizeBytes,
            Url = entity.PublicUrl,
            CreatedAt = entity.CreatedAt
        };
    }
}

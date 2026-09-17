using Fisd.Application.Models.receive.Edition;
using Fisd.Application.Models.result.Edition;
using Fisd.Application.Services.Interfaces;
using Fisd.Persistence;
using Fisd.Persistence.Entities.Content;
using Fisd.Persistence.Enums;
using Microsoft.EntityFrameworkCore;

namespace Fisd.Application.Services
{
    public class EditionsService : IEditionsService
    {
        private readonly FisdDbContext _dbContext;
        private readonly ICurrentLanguageAccessor _currentLanguageAccessor;

        public EditionsService(FisdDbContext dbContext, ICurrentLanguageAccessor currentLanguageAccessor)
        {
            _dbContext = dbContext;
            _currentLanguageAccessor = currentLanguageAccessor;
        }

        public async Task<List<EditionResultModel>> GetAllAsync()
        {
            var entities = await _dbContext.Editions.OrderByDescending(e => e.Year).ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<List<PublicEditionResultModel>> GetVisibleAsync()
        {
            var entities = await _dbContext.Editions
                .Where(e => e.IsVisible && e.Status == ContentStatusEnum.Published)
                .OrderByDescending(e => e.Year)
                .ToListAsync();
            var isEn = _currentLanguageAccessor.GetLanguage() == "en";
            return entities.Select(e => ToPublicResultModel(e, isEn)).ToList();
        }

        public async Task<PublicEditionResultModel?> GetCurrentAsync()
        {
            var entity = await _dbContext.Editions.FirstOrDefaultAsync(e => e.IsCurrent);
            if (entity == null)
            {
                return null;
            }

            var isEn = _currentLanguageAccessor.GetLanguage() == "en";
            return ToPublicResultModel(entity, isEn);
        }

        public async Task<EditionResultModel?> SetCurrentAsync(Guid id)
        {
            var entity = await _dbContext.Editions.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return null;
            }

            // Only one edition can be "current" - clear it from every other row first so this
            // never ends up with two (or zero, if the caller retries oddly) current editions.
            var previouslyCurrent = await _dbContext.Editions.Where(e => e.IsCurrent && e.Id != id).ToListAsync();
            foreach (var edition in previouslyCurrent)
            {
                edition.IsCurrent = false;
            }

            entity.IsCurrent = true;
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<List<ProgramDayResultModel>> GetDaysAsync(Guid editionId)
        {
            var entities = await _dbContext.ProgramDays
                .Where(d => d.EditionId == editionId)
                .OrderBy(d => d.DisplayOrder)
                .ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<List<ScheduleItemResultModel>> GetScheduleAsync(Guid dayId)
        {
            var entities = await _dbContext.ScheduleItems
                .Where(s => s.ProgramDayId == dayId)
                .OrderBy(s => s.DisplayOrder)
                .ToListAsync();
            return entities.Select(ToResultModel).ToList();
        }

        public async Task<EditionResultModel> CreateAsync(SaveEditionModel model)
        {
            var entity = new EditionEntity
            {
                Id = Guid.NewGuid(),
                Year = model.Year,
                BadgeFr = model.BadgeFr,
                BadgeEn = model.BadgeEn,
                TitleFr = model.TitleFr,
                TitleEn = model.TitleEn,
                TextFr = model.TextFr,
                TextEn = model.TextEn,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                LocationLabelFr = model.LocationLabelFr,
                LocationLabelEn = model.LocationLabelEn,
                TicketingUrl = model.TicketingUrl,
                IsVisible = model.IsVisible,
                Status = ParseStatus(model.Status)
            };

            _dbContext.Editions.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<EditionResultModel?> UpdateAsync(Guid id, SaveEditionModel model)
        {
            var entity = await _dbContext.Editions.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return null;
            }

            entity.Year = model.Year;
            entity.BadgeFr = model.BadgeFr;
            entity.BadgeEn = model.BadgeEn;
            entity.TitleFr = model.TitleFr;
            entity.TitleEn = model.TitleEn;
            entity.TextFr = model.TextFr;
            entity.TextEn = model.TextEn;
            entity.StartDate = model.StartDate;
            entity.EndDate = model.EndDate;
            entity.LocationLabelFr = model.LocationLabelFr;
            entity.LocationLabelEn = model.LocationLabelEn;
            entity.TicketingUrl = model.TicketingUrl;
            entity.IsVisible = model.IsVisible;
            entity.Status = ParseStatus(model.Status);

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbContext.Editions.FirstOrDefaultAsync(e => e.Id == id);
            if (entity == null)
            {
                return false;
            }

            _dbContext.Editions.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ProgramDayResultModel?> AddDayAsync(Guid editionId, SaveProgramDayModel model)
        {
            var editionExists = await _dbContext.Editions.AnyAsync(e => e.Id == editionId);
            if (!editionExists)
            {
                return null;
            }

            var entity = new ProgramDayEntity
            {
                Id = Guid.NewGuid(),
                EditionId = editionId,
                Label = model.Label,
                DateLabel = model.DateLabel,
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.ProgramDays.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<ProgramDayResultModel?> UpdateDayAsync(Guid editionId, Guid dayId, SaveProgramDayModel model)
        {
            var entity = await _dbContext.ProgramDays.FirstOrDefaultAsync(d => d.Id == dayId && d.EditionId == editionId);
            if (entity == null)
            {
                return null;
            }

            entity.Label = model.Label;
            entity.DateLabel = model.DateLabel;
            entity.DisplayOrder = model.DisplayOrder;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteDayAsync(Guid editionId, Guid dayId)
        {
            var entity = await _dbContext.ProgramDays.FirstOrDefaultAsync(d => d.Id == dayId && d.EditionId == editionId);
            if (entity == null)
            {
                return false;
            }

            _dbContext.ProgramDays.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        public async Task<ScheduleItemResultModel?> AddScheduleItemAsync(Guid dayId, SaveScheduleItemModel model)
        {
            var dayExists = await _dbContext.ProgramDays.AnyAsync(d => d.Id == dayId);
            if (!dayExists)
            {
                return null;
            }

            var entity = new ScheduleItemEntity
            {
                Id = Guid.NewGuid(),
                ProgramDayId = dayId,
                Time = model.Time,
                Title = model.Title,
                Tag = model.Tag,
                Detail = model.Detail,
                Location = model.Location,
                DisplayOrder = model.DisplayOrder
            };

            _dbContext.ScheduleItems.Add(entity);
            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<ScheduleItemResultModel?> UpdateScheduleItemAsync(Guid dayId, Guid itemId, SaveScheduleItemModel model)
        {
            var entity = await _dbContext.ScheduleItems.FirstOrDefaultAsync(s => s.Id == itemId && s.ProgramDayId == dayId);
            if (entity == null)
            {
                return null;
            }

            entity.Time = model.Time;
            entity.Title = model.Title;
            entity.Tag = model.Tag;
            entity.Detail = model.Detail;
            entity.Location = model.Location;
            entity.DisplayOrder = model.DisplayOrder;

            await _dbContext.SaveChangesAsync();
            return ToResultModel(entity);
        }

        public async Task<bool> DeleteScheduleItemAsync(Guid dayId, Guid itemId)
        {
            var entity = await _dbContext.ScheduleItems.FirstOrDefaultAsync(s => s.Id == itemId && s.ProgramDayId == dayId);
            if (entity == null)
            {
                return false;
            }

            _dbContext.ScheduleItems.Remove(entity);
            await _dbContext.SaveChangesAsync();
            return true;
        }

        private static ContentStatusEnum ParseStatus(string status) =>
            status == "published" ? ContentStatusEnum.Published : ContentStatusEnum.Draft;

        private static EditionResultModel ToResultModel(EditionEntity entity) => new()
        {
            Id = entity.Id,
            Year = entity.Year,
            BadgeFr = entity.BadgeFr,
            BadgeEn = entity.BadgeEn,
            TitleFr = entity.TitleFr,
            TitleEn = entity.TitleEn,
            TextFr = entity.TextFr,
            TextEn = entity.TextEn,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            LocationLabelFr = entity.LocationLabelFr,
            LocationLabelEn = entity.LocationLabelEn,
            TicketingUrl = entity.TicketingUrl,
            IsVisible = entity.IsVisible,
            Status = entity.Status == ContentStatusEnum.Published ? "published" : "draft",
            IsCurrent = entity.IsCurrent
        };

        private static PublicEditionResultModel ToPublicResultModel(EditionEntity entity, bool isEn) => new()
        {
            Id = entity.Id,
            Year = entity.Year,
            Badge = isEn ? entity.BadgeEn : entity.BadgeFr,
            Title = isEn ? entity.TitleEn : entity.TitleFr,
            Text = isEn ? entity.TextEn : entity.TextFr,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            LocationLabel = isEn ? entity.LocationLabelEn : entity.LocationLabelFr,
            TicketingUrl = entity.TicketingUrl,
            IsCurrent = entity.IsCurrent
        };

        private static ProgramDayResultModel ToResultModel(ProgramDayEntity entity) => new()
        {
            Id = entity.Id,
            EditionId = entity.EditionId,
            Label = entity.Label,
            DateLabel = entity.DateLabel,
            DisplayOrder = entity.DisplayOrder
        };

        private static ScheduleItemResultModel ToResultModel(ScheduleItemEntity entity) => new()
        {
            Id = entity.Id,
            ProgramDayId = entity.ProgramDayId,
            Time = entity.Time,
            Title = entity.Title,
            Tag = entity.Tag,
            Detail = entity.Detail,
            Location = entity.Location,
            DisplayOrder = entity.DisplayOrder
        };
    }
}

using Fisd.Application.Models.receive.Diagnostics;
using Fisd.Application.Models.result.Diagnostics;

namespace Fisd.Application.Services.Interfaces
{
    public interface ITicketsService
    {
        Task<List<TicketResultModel>> GetAllAsync();
        Task<List<TicketResultModel>> GetMineAsync(Guid adminUserId);
        Task<TicketResultModel> CreateAsync(Guid adminUserId, string adminEmail, SaveTicketModel model);
        Task<TicketResultModel?> UpdateStatusAsync(Guid id, UpdateTicketStatusModel model);
        Task<bool> DeleteAsync(Guid id);
        Task<TicketAttachmentResultModel?> UploadAttachmentAsync(Guid ticketId, Guid actingAdminId, bool isPlatformAdmin, Stream content, string contentType, long lengthBytes, string originalFileName);
        Task<bool> DeleteAttachmentAsync(Guid ticketId, Guid attachmentId, Guid actingAdminId, bool isPlatformAdmin);
    }
}

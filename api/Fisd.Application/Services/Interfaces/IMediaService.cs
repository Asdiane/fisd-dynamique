using Fisd.Application.Models.result.Media;

namespace Fisd.Application.Services.Interfaces
{
    public class InvalidMediaException : Exception
    {
        public InvalidMediaException(string message) : base(message)
        {
        }
    }

    public interface IMediaService
    {
        Task<List<MediaFileResultModel>> GetAllAsync();
        Task<MediaFileResultModel> UploadAsync(string category, Stream content, string contentType, long lengthBytes, string originalFileName, string? altText);
        Task<bool> DeleteAsync(Guid id);
    }
}

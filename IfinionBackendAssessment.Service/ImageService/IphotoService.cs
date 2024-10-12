using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace IfinionBackendAssessment.Service.ImageService
{
    public interface IphotoService
    {
        Task<ImageUploadResult> AddPhotoAsync(IFormFile formFile);
        Task<DeletionResult> DeletePhotoAsync(string publicId);
    }
}

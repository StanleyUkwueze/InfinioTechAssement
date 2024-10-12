using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace IfinionBackendAssessment.Service.ImageService
{
    public class PhotoService : IphotoService
    {
        private readonly Cloudinary _cloud;
        private readonly IConfiguration _con;
        public PhotoService(IOptions<CloudinarySettings> config, IConfiguration con)
        {
            _con = con;
            var acc = new Account
            {
                ApiKey = _con.GetSection("CloudinarySettings:ApiKey").Value,
                ApiSecret = _con.GetSection("CloudinarySettings:ApiSecret").Value,
                Cloud = _con.GetSection("CloudinarySettings:CloudName").Value
            };


            _cloud = new Cloudinary(acc);
        }

        public async Task<ImageUploadResult> AddPhotoAsync(IFormFile formFile)
        {
            var uploadResult = new ImageUploadResult();

            if (formFile != null && formFile.Length > 0)
            {
                using var stream = formFile.OpenReadStream();
                var UploadParams = new ImageUploadParams
                {
                    File = new FileDescription(formFile.FileName, stream),
                    Transformation = new Transformation().Height(500).Width(400).Crop("fill").Gravity("face")
                };

                uploadResult = await _cloud.UploadAsync(UploadParams);
            }
            return uploadResult;
        }

        public async Task<DeletionResult> DeletePhotoAsync(string publicId)
        {
            var deleteParams = new DeletionParams(publicId);
            var response = await _cloud.DestroyAsync(deleteParams);

            return response;
        }
    }
}

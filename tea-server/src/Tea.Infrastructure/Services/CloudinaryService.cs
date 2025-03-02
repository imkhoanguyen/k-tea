using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tea.Application.DTOs.Cloudinaries;
using Tea.Application.Interfaces;
using Tea.Infrastructure.Configurations;

namespace Tea.Infrastructure.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;
        private readonly ILogger<CloudinaryService> _logger;
        public CloudinaryService(IOptions<CloudinaryConfig> config, ILogger<CloudinaryService> logger)
        {
            var acc = new Account(config.Value.CloudName, config.Value.ApiKey, config.Value.ApiSecret);
            _cloudinary = new Cloudinary(acc);
            _logger = logger;
        }

        public async Task<FileUploadResult> AddImageAsync(IFormFile file)
        {
            // check file null or empty
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("File is null or empty");
                return new FileUploadResult
                {
                    Error = "File is null or empty"
                };
            };

            // check dinh dang file
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                _logger.LogWarning($"Invalid file format: {fileExtension}");
                return new FileUploadResult
                {
                    Error = $"Invalid file format. Allowed formats: {string.Join(", ", allowedExtensions)}"
                };
            }

            var uploadResult = new ImageUploadResult();

            using var stream = file.OpenReadStream();
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(file.FileName, stream),
                //Transformation = new Transformation(), // options: "fill", "limit", "scale"
                Folder = "k-tea"
            };

            uploadResult = await _cloudinary.UploadAsync(uploadParams);



            if (uploadResult.Error != null)
            {
                _logger.LogError($"Failed to upload file to Cloudinary. Error: {uploadResult.Error.Message}");
                return new FileUploadResult
                {
                    Error = uploadResult.Error.Message
                };
            }

            _logger.LogInformation($"File uploaded successfully. PublicId: {uploadResult.PublicId}, Url: {uploadResult.SecureUrl}");

            return new FileUploadResult
            {
                PublicId = uploadResult.PublicId,
                Url = uploadResult.SecureUrl.ToString(),
                Error = null
            };
        }

        public async Task<FileDeleteResult> DeleteFileAsync(string publicId)
        {
            if (string.IsNullOrEmpty(publicId))
            {
                _logger.LogWarning("Failed to delete file from Cloudinary: PublicId is null or empty.");
                return new FileDeleteResult
                {
                    Error = "PublicId is null or empty."
                };
            }

            var deleteParams = new DeletionParams(publicId);
            var result = await _cloudinary.DestroyAsync(deleteParams);

            if (result.Error != null)
            {
                _logger.LogWarning($"Failed to delete file from Cloudinary. PublicId: {publicId}, Error: {result.Error.Message}");
                return new FileDeleteResult
                {
                    Error = result.Error.Message
                };
            }

            _logger.LogInformation($"Successfully deleted file from Cloudinary. PublicId: {publicId}");

            return new FileDeleteResult
            {
                Result = result.Result,
                Error = null
            };
        }
    }
}

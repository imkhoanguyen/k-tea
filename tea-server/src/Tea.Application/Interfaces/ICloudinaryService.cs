using Microsoft.AspNetCore.Http;
using Tea.Application.DTOs.Cloudinaries;

namespace Tea.Application.Interfaces
{
    public interface ICloudinaryService
    {
        Task<FileUploadResult> AddImageAsync(IFormFile file);
        Task<FileDeleteResult> DeleteFileAsync(string publicId);
    }
}

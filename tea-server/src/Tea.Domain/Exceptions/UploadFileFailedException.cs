namespace Tea.Domain.Exceptions
{
    public class UploadFileFailedException : BadRequestException
    {
        public UploadFileFailedException(string? message = null) : base($"Failed to upload the file to Cloudinary. {message}")
        {
        }
    }
}

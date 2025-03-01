namespace Tea.Domain.Exceptions
{
    public class UploadFileFailedException : BadRequestException
    {
        public UploadFileFailedException() : base("Failed to upload the file to Cloudinary.")
        {
        }
    }
}

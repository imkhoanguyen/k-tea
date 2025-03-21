using Tea.Domain.Exceptions.Bases;

namespace Tea.Domain.Exceptions.BadRequests
{
    public class UploadFileFailedException : BadRequestException
    {
        public UploadFileFailedException(string? message = null) : base($"Có lỗi xảy ra khi upload hinhg ảnh. {message}")
        {
        }
    }
}

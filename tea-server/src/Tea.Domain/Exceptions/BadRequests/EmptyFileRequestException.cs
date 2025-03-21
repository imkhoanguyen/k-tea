using Tea.Domain.Exceptions.Bases;

namespace Tea.Domain.Exceptions.BadRequests
{
    public class EmptyFileRequestException : BadRequestException
    {
        public EmptyFileRequestException() : base("Bạn chưa chọn file upload")
        {
        }
    }
}

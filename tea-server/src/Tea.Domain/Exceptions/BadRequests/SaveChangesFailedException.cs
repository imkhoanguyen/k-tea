using Tea.Domain.Exceptions.Bases;

namespace Tea.Domain.Exceptions.BadRequests
{
    public sealed class SaveChangesFailedException : BadRequestException
    {
        public SaveChangesFailedException() : base("Lưu thay đổi thất bại. Vui lòng thử lại sau")
        {
        }
    }
}

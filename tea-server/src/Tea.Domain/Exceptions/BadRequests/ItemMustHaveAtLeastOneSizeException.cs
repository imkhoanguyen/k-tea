using Tea.Domain.Exceptions.Bases;

namespace Tea.Domain.Exceptions.BadRequests
{
    public class ItemMustHaveAtLeastOneSizeException : BadRequestException
    {
        public ItemMustHaveAtLeastOneSizeException() : base("Mỗi sản phẩm phải có ít nhất 1 size.")
        {
        }
    }
}

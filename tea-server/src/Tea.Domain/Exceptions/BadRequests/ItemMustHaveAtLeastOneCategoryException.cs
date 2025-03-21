using Tea.Domain.Exceptions.Bases;

namespace Tea.Domain.Exceptions.BadRequests
{
    public class ItemMustHaveAtLeastOneCategoryException : BadRequestException
    {
        public ItemMustHaveAtLeastOneCategoryException() : base("Mỗi sản phẩm phải có ít nhất 1 danh mục.")
        {
        }
    }
}

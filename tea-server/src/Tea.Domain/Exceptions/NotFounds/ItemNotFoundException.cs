using Tea.Domain.Exceptions.Bases;

namespace Tea.Domain.Exceptions.NotFounds
{
    public class ItemNotFoundException : NotFoundException
    {
        public ItemNotFoundException(int id) : base($"Không tìm thấy sản phẩm với id: {id}.")
        {
        }
    }
}

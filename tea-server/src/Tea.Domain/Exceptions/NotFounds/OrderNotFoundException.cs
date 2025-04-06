namespace Tea.Domain.Exceptions.NotFounds
{
    public class OrderNotFoundException : NotFoundException
    {
        public OrderNotFoundException(int id) : base($"Không tìm thấy đơn hàng với id: {id}.")
        {
        }
    }
}

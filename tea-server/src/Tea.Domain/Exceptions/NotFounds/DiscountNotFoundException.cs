namespace Tea.Domain.Exceptions.NotFounds
{
    public class DiscountNotFoundException : NotFoundException
    {
        public DiscountNotFoundException(string code) : base($"Không tìm thấy mã giảm giá với mã là: {code}.")
        {
        }

        public DiscountNotFoundException(int id) : base($"Không tìm thấy mã giảm giá với id là: {id}.")
        {
        }
    }
}

namespace Tea.Domain.Exceptions.BadRequests
{
    public class DiscountHasExpired : BadRequestException
    {
        public DiscountHasExpired() : base("Mã giảm giá đã hết hạn.")
        {
        }
    }
}

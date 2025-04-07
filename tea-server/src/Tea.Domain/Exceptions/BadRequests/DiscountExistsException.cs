namespace Tea.Domain.Exceptions.BadRequests
{
    public class DiscountExistsException : BadRequestException
    {
        public DiscountExistsException(string message) : base(message)
        {
        }
    }
}

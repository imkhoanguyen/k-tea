namespace Tea.Domain.Exceptions.BadRequests
{
    public class EmptyDiscountIdListException : BadRequestException
    {
        public EmptyDiscountIdListException(string message) : base(message)
        {
        }
    }
}

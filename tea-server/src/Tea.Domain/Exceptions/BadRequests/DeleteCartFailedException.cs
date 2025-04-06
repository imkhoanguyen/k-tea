namespace Tea.Domain.Exceptions.BadRequests
{
    public class DeleteCartFailedException : BadRequestException
    {
        public DeleteCartFailedException(string message) : base(message)
        {
        }
    }
}

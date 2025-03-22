namespace Tea.Domain.Exceptions.BadRequests
{
    public class DeleteClaimFailedException : BadRequestException
    {
        public DeleteClaimFailedException(string message) : base(message)
        {
        }
    }
}

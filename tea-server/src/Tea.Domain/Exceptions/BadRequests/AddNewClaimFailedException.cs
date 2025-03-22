namespace Tea.Domain.Exceptions.BadRequests
{
    public class AddNewClaimFailedException : BadRequestException
    {
        public AddNewClaimFailedException(string message) : base(message)
        {
        }
    }
}

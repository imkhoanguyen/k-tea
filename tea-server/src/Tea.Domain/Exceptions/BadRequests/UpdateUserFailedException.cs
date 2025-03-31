namespace Tea.Domain.Exceptions.BadRequests
{
    public class UpdateUserFailedException : BadRequestException
    {
        public UpdateUserFailedException(string message) : base(message)
        {
        }
    }
}

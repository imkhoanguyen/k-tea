namespace Tea.Domain.Exceptions.BadRequests
{
    public class DeleteUserFailedException : BadRequestException
    {
        public DeleteUserFailedException(string message) : base(message)
        {
        }
    }
}

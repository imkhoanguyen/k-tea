namespace Tea.Domain.Exceptions.BadRequests
{
    public class UpdatePasswordFailedException : BadRequestException
    {
        public UpdatePasswordFailedException(string message) : base(message)
        {
        }
    }
}

namespace Tea.Domain.Exceptions.BadRequests
{
    public class ResetPasswordFailedException : BadRequestException
    {
        public ResetPasswordFailedException(string message) : base(message)
        {
        }
    }
}

namespace Tea.Domain.Exceptions.BadRequests
{
    public class SendMailFailedException : BadRequestException
    {
        public SendMailFailedException(string message) : base(message)
        {
        }
    }
}

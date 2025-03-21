namespace Tea.Domain.Exceptions.BadRequests
{
    public class AddNewUserFailedException : BadRequestException
    {
        public AddNewUserFailedException(string message) : base(message)
        {
        }
    }
}

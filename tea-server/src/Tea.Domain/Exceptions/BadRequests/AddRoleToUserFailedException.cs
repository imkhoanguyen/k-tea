namespace Tea.Domain.Exceptions.BadRequests
{
    public class AddRoleToUserFailedException : BadRequestException
    {
        public AddRoleToUserFailedException(string message) : base(message)
        {
        }
    }
}

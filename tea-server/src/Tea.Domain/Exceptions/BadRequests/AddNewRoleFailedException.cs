namespace Tea.Domain.Exceptions.BadRequests
{
    public class AddNewRoleFailedException : BadRequestException
    {
        public AddNewRoleFailedException(string message) : base(message)
        {
        }
    }
}

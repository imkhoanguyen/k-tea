namespace Tea.Domain.Exceptions.BadRequests
{
    public class DeleteRoleFailedException : BadRequestException
    {
        public DeleteRoleFailedException(string message) : base(message)
        {
        }
    }
}

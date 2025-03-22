namespace Tea.Domain.Exceptions.BadRequests
{
    public class UpdateRoleFailedException : BadRequestException
    {
        public UpdateRoleFailedException(string message) : base(message)
        {
        }
    }
}

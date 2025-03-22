namespace Tea.Domain.Exceptions.BadRequests
{
    public class RoleExistsException : BadRequestException
    {
        public RoleExistsException(string message) : base(message)
        {
        }
    }
}

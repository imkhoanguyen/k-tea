namespace Tea.Domain.Exceptions.BadRequests
{
    public class PasswordNotCorretException : BadRequestException
    {
        public PasswordNotCorretException(string message) : base(message)
        {
        }
    }
}

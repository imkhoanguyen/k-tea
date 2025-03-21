namespace Tea.Domain.Exceptions.BadRequests
{
    public class WrongPasswordException : BadRequestException
    {
        public WrongPasswordException() : base("Sai mật khẩu. Vui lòng thử lại.")
        {
        }
    }
}

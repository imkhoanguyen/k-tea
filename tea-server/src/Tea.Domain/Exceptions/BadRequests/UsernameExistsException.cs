namespace Tea.Domain.Exceptions.BadRequests
{
    public class UsernameExistsException : BadRequestException
    {
        public UsernameExistsException() : base("Username đã tồn tại. Vui lòng nhập username khác.")
        {
        }
    }
}

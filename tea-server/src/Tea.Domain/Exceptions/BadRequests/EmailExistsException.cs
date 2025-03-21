namespace Tea.Domain.Exceptions.BadRequests
{
    public class EmailExistsException : BadRequestException
    {
        public EmailExistsException() : base("Email đã tồn tại. Vui lòng nhập email khác.")
        {
        }
    }
}

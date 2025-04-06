namespace Tea.Domain.Exceptions.NotFounds
{
    public class UserNotFoundException : NotFoundException
    {
        public UserNotFoundException(string userName) : base($"Không tìm thấy người dùng với username/email: {userName}.")
        {
        }
    }
}

namespace Tea.Domain.Exceptions.NotFounds
{
    public class RoleNotFoundException : NotFoundException
    {
        public RoleNotFoundException(string message) : base($"Không tìm thấy quyền với id: {message}")
        {
        }
    }
}

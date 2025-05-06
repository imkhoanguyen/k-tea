namespace Tea.Application.DTOs.Users
{
    public class UserResponse
    {
        public required string Id { get; set; }
        public required string FullName { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public required string UserName { get; set; }
        public bool IsLooked { get; set; } = false;
        public required string Role { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
    }
}

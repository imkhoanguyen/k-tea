using System.ComponentModel.DataAnnotations;

namespace Tea.Application.DTOs.Users
{
    public class RegisterRequest
    {
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string FullName { get; set; }
        public required string PhoneNumber { get; set; }
    }
}

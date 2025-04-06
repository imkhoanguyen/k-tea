using Microsoft.AspNetCore.Identity;

namespace Tea.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public required string FullName { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
        public required string Address { get; set; }
        public bool IsDeleted { get; set; }
    }
}

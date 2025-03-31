using Microsoft.AspNetCore.Identity;

namespace Tea.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public required string FullName { get; set; }
        public DateTimeOffset Created { get; set; } = DateTimeOffset.Now;
    }
}

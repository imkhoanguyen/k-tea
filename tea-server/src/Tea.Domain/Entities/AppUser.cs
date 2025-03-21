using Microsoft.AspNetCore.Identity;

namespace Tea.Domain.Entities
{
    public class AppUser : IdentityUser
    {
        public required string FullName { get; set; }
        public required string ImgUrl { get; set; }
        public string? PublicId { get; set; }
    }
}

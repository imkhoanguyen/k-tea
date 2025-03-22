using Microsoft.AspNetCore.Identity;

namespace Tea.Domain.Entities
{
    public class AppRole : IdentityRole
    {
        public string Description { get; set; } = string.Empty;
    }
}

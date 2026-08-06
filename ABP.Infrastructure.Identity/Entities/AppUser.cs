using Microsoft.AspNetCore.Identity;

namespace ABP.Infrastructure.Identity.Entities
{
    public class AppUser : IdentityUser
    {
        public required string Name { get; set; }
        public required string LastName { get; set; }
        public string? ProfileImage { get; set; }
        public bool IsActive { get; set; } = false;
    }
}

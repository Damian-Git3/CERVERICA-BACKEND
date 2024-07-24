using Microsoft.AspNetCore.Identity;

namespace CERVERICA.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        [Required]
        public bool Activo { get; set; }
        public DateTime? FechaRegistro { get; set; } = DateTime.Now;
    }
}

using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class RegisterDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string FullName { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;
        public string Telefono { get; set; } = string.Empty;

        public string? Role { get; set; }
    }
}

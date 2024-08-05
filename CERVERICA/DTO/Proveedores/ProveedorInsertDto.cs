using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class ProveedorInsertDto
    {
        [Required]
        [StringLength(100)]
        public string Empresa { get; set; }

        [Required]
        [Phone]
        public string Telefono { get; set; }

        [Required]
        [StringLength(45)]
        public string Direccion { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(45)]
        public string NombreContacto { get; set; }
    }
}

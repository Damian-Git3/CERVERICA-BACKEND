using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class ProveedoresDto
    {
        public int Id { get; set; } // ID único del proveedor

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la empresa no puede exceder los 100 caracteres.")]
        public string Empresa { get; set; } // Nombre de la empresa del proveedor

        [Required(ErrorMessage = "El número de teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El número de teléfono no tiene un formato válido.")]
        public string Telefono { get; set; } // Número de teléfono del proveedor

        [Required(ErrorMessage = "La dirección es obligatoria.")]
        [StringLength(45, ErrorMessage = "La dirección no puede exceder los 45 caracteres.")]
        public string Direccion { get; set; } // Dirección del proveedor

        [StringLength(100, ErrorMessage = "El email no puede exceder los 100 caracteres.")]
        [EmailAddress(ErrorMessage = "El email no tiene un formato válido.")]
        public string Email { get; set; } // Email del proveedor, opcional

        [Required(ErrorMessage = "El nombre de contacto es obligatorio.")]
        [StringLength(45, ErrorMessage = "El nombre de contacto no puede exceder los 45 caracteres.")]
        public string NombreContacto { get; set; } // Nombre del contacto del proveedor

        [Required]
        public bool Activo { get; set; } // Estado activo del proveedor (true/false)
    }
}

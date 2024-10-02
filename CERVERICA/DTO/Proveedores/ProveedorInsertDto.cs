using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class ProveedorInsertDto
    {
        [Required(ErrorMessage = "El campo Empresa es obligatorio.")]
        [StringLength(100, ErrorMessage = "El campo Empresa no puede exceder los 100 caracteres.")]
        public string Empresa { get; set; }

        [Required(ErrorMessage = "El campo Teléfono es obligatorio.")]
        [Phone(ErrorMessage = "El formato del Teléfono no es válido.")]
        public string Telefono { get; set; }

        [Required(ErrorMessage = "El campo Dirección es obligatorio.")]
        [StringLength(45, ErrorMessage = "El campo Dirección no puede exceder los 45 caracteres.")]
        public string Direccion { get; set; }

        [StringLength(100, ErrorMessage = "El campo Email no puede exceder los 100 caracteres.")]
        [EmailAddress(ErrorMessage = "El formato del Email no es válido.")]
        public string Email { get; set; }

        [Required(ErrorMessage = "El campo Nombre de Contacto es obligatorio.")]
        [StringLength(45, ErrorMessage = "El campo Nombre de Contacto no puede exceder los 45 caracteres.")]
        public string NombreContacto { get; set; }
    }
}

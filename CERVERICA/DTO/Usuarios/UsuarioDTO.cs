using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.Usuarios
{
    public class UsuarioDTO
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public string Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El correo es obligatorio.")]
        [EmailAddress(ErrorMessage = "El formato del correo no es válido.")]
        public string Correo { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio.")]
        public string Rol { get; set; }

        [Required(ErrorMessage = "El estado del usuario es obligatorio.")]
        public bool Activo { get; set; }
    }
}

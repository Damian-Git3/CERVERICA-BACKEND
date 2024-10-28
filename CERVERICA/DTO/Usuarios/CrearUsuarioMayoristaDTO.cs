using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.Usuarios
{
    public class CrearUsuarioMayoristaDto
    {
        // DATOS DEL USUARIO}

        [Required(ErrorMessage = "La contraseña es obligatoria")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "La contraseña debe tener entre 6 y 100 caracteres")]
        public string Password { get; set; }

        [Required(ErrorMessage = "El rol es obligatorio")]
        public string Rol { get; set; }

        // DATOS DEL CLIENTE MAYORISTA Y EMPRESA

        [Required(ErrorMessage = "El nombre de la empresa es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre de la empresa no puede exceder los 100 caracteres")]
        public string NombreEmpresa { get; set; }

        [Required(ErrorMessage = "La dirección de la empresa es obligatoria")]
        [MaxLength(200, ErrorMessage = "La dirección de la empresa no puede exceder los 200 caracteres")]
        public string DireccionEmpresa { get; set; }

        [Required(ErrorMessage = "El teléfono de la empresa es obligatorio")]
        [Phone(ErrorMessage = "Número de teléfono no válido")]
        public string TelefonoEmpresa { get; set; }

        [Required(ErrorMessage = "El email de la empresa es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string EmailEmpresa { get; set; }

        [Required(ErrorMessage = "El RFC de la empresa es obligatorio")]
        [MaxLength(13, ErrorMessage = "El RFC de la empresa no puede exceder los 13 caracteres")]
        public string RFCEmpresa { get; set; }

        [Required(ErrorMessage = "El nombre del contacto es obligatorio")]
        [MaxLength(100, ErrorMessage = "El nombre del contacto no puede exceder los 100 caracteres")]
        public string NombreContacto { get; set; }

        [MaxLength(100, ErrorMessage = "El cargo del contacto no puede exceder los 100 caracteres")]
        public string CargoContacto { get; set; }

        [Required(ErrorMessage = "El teléfono del contacto es obligatorio")]
        [Phone(ErrorMessage = "Número de teléfono no válido")]
        public string TelefonoContacto { get; set; }

        [Required(ErrorMessage = "El email del contacto es obligatorio")]
        [EmailAddress(ErrorMessage = "Email no válido")]
        public string EmailContacto { get; set; }

    }

}

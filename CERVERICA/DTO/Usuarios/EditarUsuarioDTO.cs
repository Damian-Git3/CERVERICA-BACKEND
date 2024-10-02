namespace CERVERICA.DTO.Usuarios
{
    public class EditarUsuarioDTO
    {
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public string? Password { get; set; }
    }
}

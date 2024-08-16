namespace CERVERICA.DTO.Usuarios
{
    public class UsuarioDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
    }
}

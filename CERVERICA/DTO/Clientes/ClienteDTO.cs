namespace CERVERICA.DTO.Clientes

{
    public class ClienteDTO
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Correo { get; set; }
        public string Rol { get; set; }
        public bool Activo { get; set; }
        public DateTime? FechaRegistro { get; set; }
    }
}

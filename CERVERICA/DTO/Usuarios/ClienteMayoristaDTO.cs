using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.Usuarios
{
    public class ClienteMayoristaDTO
    {
        public string Id { get; set; }

        public string NombreEmpresa { get; set; }
        
        public string DireccionEmpresa { get; set; }
        
        public string TelefonoEmpresa { get; set; }
        
        public string EmailEmpresa { get; set; }
        
        public string NombreContacto { get; set; }
        
        public string CargoContacto { get; set; }
        
        public string TelefonoContacto { get; set; }
        
        public string EmailContacto { get; set; }
    }
}

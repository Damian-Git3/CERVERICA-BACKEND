using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.Agente
{
    public class ActualizarSolicitudCambioAgenteDTO
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public int IdMayorista { get; set; }

        [Required]
        public string Estatus { get; set; }
        [Required]
        public string IdAdministrador { get; set; }
        [Required]
        public string IdAgenteActual { get; set; }
        
        public string? IdAgenteNuevo { get; set; }

        public string? MotivoRechazo { get; set; }
    }
}

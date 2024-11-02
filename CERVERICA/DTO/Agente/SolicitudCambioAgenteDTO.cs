using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.Agente
{
    public class SolicitudCambioAgenteDTO
    {
        [Required]
        public string IdAgenteVentaActual { get; set; }

        [Required]
        public string Motivo { get; set; }

        [Required]
        public int Solicitante { get; set; } // 1 cliente 2 agente 3 admin

        [Required]
        public int IdMayorista { get; set; }
    }
}

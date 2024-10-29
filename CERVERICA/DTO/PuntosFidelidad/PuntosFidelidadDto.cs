using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.PuntosFidelidad
{
    public class PuntosFidelidadDto
    {
        public int Id { get; set; }
        public string IdUsuario { get; set; }
        public int PuntosAcumulados { get; set; }
        public int PuntosRedimidos { get; set; }
        public int PuntosDisponibles { get; set; }
        public DateTime? FechaUltimaActualizacion { get; set; }
    }

}

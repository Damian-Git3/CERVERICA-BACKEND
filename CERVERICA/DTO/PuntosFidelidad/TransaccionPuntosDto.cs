using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.PuntosFidelidad
{
    public class TransaccionPuntosDto
    {
        public int Id { get; set; }
        public int Puntos { get; set; }
        public string TipoTransaccion { get; set; }
        public DateTime FechaTransaccion { get; set; }
        public string Descripcion { get; set; }
    }

}

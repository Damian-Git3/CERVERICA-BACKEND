using CERVERICA.Models;

namespace CERVERICA.Dtos
{
    public class ProduccionesDto
    {
        public int Id { get; set; }
        public DateTime FechaProduccion { get; set; }
        public DateTime FechaProximoPaso { get; set; }
        public string Mensaje { get; set; }
        public int Estatus { get; set; }
        public int NumeroTandas { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public int IdReceta { get; set; }
        public RecetasDto Receta { get; set; }
        public string NombreReceta { get; set; }
        public int Paso { get; set; }
        public string DescripcionPaso { get; set; }
        public string IdUsuario { get; set; }
        public string NombreUsuario { get; set; }


    }
}

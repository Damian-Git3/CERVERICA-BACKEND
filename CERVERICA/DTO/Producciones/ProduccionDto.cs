using CERVERICA.Dtos;

namespace CERVERICA.Dtos
{
    public class ProduccionDto
    {
        public int Id { get; set; }
        public DateTime FechaProduccion { get; set; }
        public string Mensaje { get; set; }
        public int Estatus { get; set; }
        public int NumeroTandas { get; set; }
        public int IdReceta { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public string IdUsuarioSolicitud { get; set; }
        public string IdUsuarioProduccion { get; set; }
        public int Paso { get; set; }
        public RecetaProduccionDto Receta { get; set; }
        public List<ProduccionLoteInsumoDto> ProduccionLoteInsumos { get; set; }
    }
}

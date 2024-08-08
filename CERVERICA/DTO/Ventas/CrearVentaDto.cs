using CERVERICA.Dtos;

namespace CERVERICA.Dtos
{
    public class CrearVentaDto
    {
        public int TipoVenta { get; set; }
        public List<DetalleVentaDto> Detalles { get; set; }
    }
}

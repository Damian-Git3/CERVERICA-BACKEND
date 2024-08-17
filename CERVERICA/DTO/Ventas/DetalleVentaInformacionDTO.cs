using CERVERICA.DTO.Stock;
using CERVERICA.Models;

namespace CERVERICA.DTO.Ventas
{
    public class DetalleVentaInformacionDTO
    {
        public int Id { get; set; }
        public int? Cantidad { get; set; }
        public int? Pack { get; set; }
        public int IdStock { get; set; }
        public float MontoVenta { get; set; }
        public float? CostoUnitario { get; set; }
        public StockDTO Stock { get; set; }
    }
}

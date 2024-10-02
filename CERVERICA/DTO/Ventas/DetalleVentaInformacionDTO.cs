using System.ComponentModel.DataAnnotations;
using CERVERICA.DTO.Stock;
using CERVERICA.Models;

namespace CERVERICA.DTO.Ventas
{
    public class DetalleVentaInformacionDTO
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int? Cantidad { get; set; }

        [Required(ErrorMessage = "El paquete es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El paquete debe ser al menos 1.")]
        public int? Pack { get; set; }

        [Required(ErrorMessage = "El ID de stock es obligatorio.")]
        public int IdStock { get; set; }

        [Required(ErrorMessage = "El monto de venta es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto de venta debe ser mayor que 0.")]
        public float MontoVenta { get; set; }

        public float? CostoUnitario { get; set; }

        [Required(ErrorMessage = "El stock es obligatorio.")]
        public StockDTO Stock { get; set; }
    }
}

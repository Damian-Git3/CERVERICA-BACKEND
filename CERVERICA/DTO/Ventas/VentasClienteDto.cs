using CERVERICA.Models;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class VentasClienteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de venta es obligatoria.")]
        public DateTime FechaVenta { get; set; }

        [Required(ErrorMessage = "El total es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor que cero.")]
        public float Total { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        public MetodoPago MetodoPago { get; set; }

        [Required(ErrorMessage = "El método de envío es obligatorio.")]
        public MetodoEnvio MetodoEnvio { get; set; }

        [Required(ErrorMessage = "El estatus de la venta es obligatorio.")]
        public EstatusVenta EstatusVenta { get; set; }
    }
}

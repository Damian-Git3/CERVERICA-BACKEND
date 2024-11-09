using System.ComponentModel.DataAnnotations;
using CERVERICA.Models;

namespace CERVERICA.DTO.Ventas
{
    public class PedidoDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de venta es obligatoria.")]
        public DateTime FechaVenta { get; set; }

        [Required(ErrorMessage = "El total de cervezas es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total de cervezas debe ser mayor que cero.")]
        public int TotalCervezas { get; set; }
        public float Total { get; set; }

        [Required(ErrorMessage = "El monto de venta es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El monto de venta debe ser mayor que cero.")]
        public float MontoVenta { get; set; }

        [Required(ErrorMessage = "El número de tarjeta es obligatorio.")]
        [StringLength(16, MinimumLength = 16, ErrorMessage = "El número de tarjeta debe tener 16 dígitos.")]
        [RegularExpression(@"^\d{16}$", ErrorMessage = "El número de tarjeta debe contener solo dígitos.")]
        public string NumeroTarjeta { get; set; }

        [Required(ErrorMessage = "El método de pago es obligatorio.")]
        public MetodoPago MetodoPago { get; set; }

        [Required(ErrorMessage = "El método de envío es obligatorio.")]
        public MetodoEnvio MetodoEnvio { get; set; }

        [Required(ErrorMessage = "El estatus de la venta es obligatorio.")]
        public EstatusVenta EstatusVenta { get; set; }

        [Required(ErrorMessage = "Los productos en el pedido son obligatorios.")]
        public DetalleVentaInformacionDTO[] ProductosPedido { get; set; }
    }
}

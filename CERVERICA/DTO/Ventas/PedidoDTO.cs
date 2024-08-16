using CERVERICA.Models;

namespace CERVERICA.DTO.Ventas
{
    public class PedidoDTO
    {
        public int Id { get; set; }
        public DateTime FechaVenta { get; set; }
        public float TotalCervezas { get; set; }
        public float MontoVenta { get; set; }
        public string NumeroTarjeta { get; set; }
        public MetodoPago MetodoPago { get; set; }
        public MetodoEnvio MetodoEnvio { get; set; }
        public EstatusVenta EstatusVenta { get; set; }
        public DetalleVenta[] ProductosPedido { get; set; }
    }
}

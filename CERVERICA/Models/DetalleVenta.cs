using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class DetalleVenta
    {
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(Venta))]
        public int IdVenta { get; set; }
        public Venta Venta { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(Stock))]
        public int IdStock { get; set; }
        public Stock Stock { get; set; }

        [Required]
        public float MontoVenta { get; set; }

        public int? Cantidad { get; set; }
    }
}

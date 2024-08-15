using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class DetalleVenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Venta))]
        public int IdVenta { get; set; }
        public Venta Venta { get; set; }

        [ForeignKey(nameof(Stock))]
        public int IdStock { get; set; }
        public Stock Stock { get; set; }

        [Required]
        public float MontoVenta { get; set; }

        public int? Cantidad { get; set; }

        public int? Pack { get; set; }
    }
}

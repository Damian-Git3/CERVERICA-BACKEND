using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    [Table("lotes_insumos", Schema = "cerverica")]
    public class LoteInsumo
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(Proveedor))]
        public int IdProveedor { get; set; }
        public Proveedor Proveedor { get; set; }

        [ForeignKey(nameof(Insumo))]
        public int IdInsumo { get; set; }
        public Insumo Insumo { get; set; }

        [Required]
        public DateTime FechaCaducidad { get; set; }

        [Required]
        public float Cantidad { get; set; }

        [Required]
        public DateTime FechaCompra { get; set; }

        [Required]
        public float PrecioUnidad { get; set; }
    }
}

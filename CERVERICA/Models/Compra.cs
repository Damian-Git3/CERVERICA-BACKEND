using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    [Table("compras", Schema = "cerverica")]
    public class Compra
    {
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(Usuario))]
        public string IdUsuario { get; set; }
        public ApplicationUser Usuario { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(LoteInsumo))]
        public int LotesInsumosId { get; set; }
        public LoteInsumo LoteInsumo { get; set; }

        [Key, Column(Order = 2)]
        public int LotesInsumosIdProveedor { get; set; }

        [Key, Column(Order = 3)]
        public int LotesInsumosIdInsumo { get; set; }

        [Required]
        public float PagoProveedor { get; set; }
    }
}

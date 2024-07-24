using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    [Table("ventas", Schema = "cerverica")]
    public class Venta
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public string IdUsuario { get; set; }
        public ApplicationUser Usuario { get; set; }

        [Required]
        public DateTime FechaVenta { get; set; }

        [Required]
        public float Total { get; set; }

        [Required]
        public int TipoVenta { get; set; }
    }
}

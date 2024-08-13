using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class Venta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public string IdUsuario { get; set; }

        [Required]
        public DateTime FechaVenta { get; set; }

        [Required]
        public float Total { get; set; }

        [Required]
        public int TipoVenta { get; set; }
    }
}

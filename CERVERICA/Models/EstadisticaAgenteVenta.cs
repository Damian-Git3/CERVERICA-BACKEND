using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class EstadisticaAgenteVenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(AgenteVenta))]
        public string IdAgenteVenta { get; set; }
        public virtual ApplicationUser AgenteVenta { get; set; }

        [Required]
        public float PromedioValoraciones { get; set; }


        [Required]
        public int MontoVentas { get; set; }

        [Required]
        public float FrecuenciaCambioAgente { get; set; }

    }
}

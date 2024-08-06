using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class ProduccionLoteInsumo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Produccion))]
        public int IdProduccion { get; set; }
        public Produccion Produccion { get; set; }

        [Required]
        [ForeignKey(nameof(LoteInsumo))]
        public int IdLoteInsumo { get; set; }
        public LoteInsumo LoteInsumo { get; set; }

        [Required]
        public float Cantidad { get; set; }
    }
}

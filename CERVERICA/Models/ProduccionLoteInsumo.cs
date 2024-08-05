using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Models
{
    public class ProduccionLoteInsumo
    {
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(Receta))]
        public int IdReceta { get; set; }
        public Receta Receta { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(Produccion))]
        public int IdProduccion { get; set; }
        public Produccion Produccion { get; set; }

        [Key, Column(Order = 2)]
        [ForeignKey(nameof(LoteInsumo))]
        public int IdLoteInsumo { get; set; }
        public LoteInsumo LoteInsumo { get; set; }

        [Required]
        public float Cantidad { get; set; }
    }
}

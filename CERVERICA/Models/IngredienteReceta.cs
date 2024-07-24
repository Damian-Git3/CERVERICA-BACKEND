using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    [Table("ingredientes_receta", Schema = "cerverica")]
    public class IngredienteReceta
    {
        [Key, Column(Order = 0)]
        [ForeignKey(nameof(Receta))]
        public int IdReceta { get; set; }
        public Receta Receta { get; set; }

        [Key, Column(Order = 1)]
        [ForeignKey(nameof(Insumo))]
        public int IdInsumo { get; set; }
        public Insumo Insumo { get; set; }

        [Required]
        public float Cantidad { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    [Table("insumos", Schema = "cerverica")]
    public class Insumo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(45)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(45)]
        public string Descripcion { get; set; }

        [Required]
        public float UnidadMedida { get; set; }

        [Required]
        public float CantidadMaxima { get; set; }

        [Required]
        public float CantidadMinima { get; set; }

        [Required]
        public float Merma { get; set; }

        public ICollection<LoteInsumo> LotesInsumos { get; set; }
        public ICollection<IngredienteReceta> IngredientesReceta { get; set; }
    }
}

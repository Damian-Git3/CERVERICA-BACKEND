using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class Insumo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(45)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(450)]
        public string Descripcion { get; set; }

        [Required]
        public string UnidadMedida { get; set; }

        [Required]
        public float CantidadMaxima { get; set; }

        [Required]
        public float CantidadMinima { get; set; }

        [Required]
        public float Merma { get; set; }

        public float CostoUnitario { get; set; } = 0;

        [Required]
        public bool Activo { get; set; }

        public ICollection<LoteInsumo> LotesInsumos { get; set; }
        public ICollection<IngredienteReceta> IngredientesReceta { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    [Table("receta", Schema = "cerverica")]
    public class Receta
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public float LitrosEstimados { get; set; }

        public float? Utilidad { get; set; }

        [Required]
        public int PiezasEstimadas { get; set; }

        public string Descripcion { get; set; }

        [Required]
        [StringLength(50)]
        public string Nombre { get; set; }

        public ICollection<IngredienteReceta> IngredientesReceta { get; set; }
        public ICollection<Produccion> Producciones { get; set; }
        public ICollection<Stock> Stocks { get; set; }
    }
}

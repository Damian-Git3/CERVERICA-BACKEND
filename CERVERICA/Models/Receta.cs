using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class Receta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public float LitrosEstimados { get; set; }

        public float? PrecioLitro { get; set; }

        public string Descripcion { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        public float CostoProduccion { get; set; }

        [Required]
        public string Imagen { get; set; }

        [Required]
        public bool Activo { get; set; }

        public ICollection<IngredienteReceta> IngredientesReceta { get; set; }
        public ICollection<Produccion> Producciones { get; set; }
        public ICollection<Stock> Stocks { get; set; }
        public ICollection<PasosReceta> PasosReceta { get; set; }
        public ICollection<ProduccionLoteInsumo> ProduccionLoteInsumos { get; set; }
    }
}

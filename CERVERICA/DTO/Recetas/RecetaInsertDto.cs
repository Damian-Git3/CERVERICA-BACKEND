using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class RecetaInsertDto
    {
        [Required]
        public float LitrosEstimados { get; set; }

        public string Descripcion { get; set; }

        public string Especificaciones { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        public string Imagen { get; set; }

        [Required]
        public string RutaFondo { get; set; }

        public List<IngredienteRecetaInsertDto> IngredientesReceta { get; set; }
    }
}

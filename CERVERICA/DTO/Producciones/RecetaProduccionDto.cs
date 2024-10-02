using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CERVERICA.Dtos
{
    public class RecetaProduccionDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre de la receta es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la receta no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Debe especificar al menos un ingrediente para la receta.")]
        public List<IngredienteRecetaDetalleDto> IngredientesReceta { get; set; }
    }
}

using CERVERICA.Models;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.Stock
{
    public class StockDTO
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }

        [Required(ErrorMessage = "La receta asociada es obligatoria.")]
        public Receta Receta { get; set; }
    }
}

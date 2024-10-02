using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class SolicitarProduccionDto
    {
        [Required(ErrorMessage = "El número de tandas es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de tandas debe ser mayor a 0.")]
        public int NumeroTandas { get; set; }

        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        [StringLength(450, ErrorMessage = "El ID del usuario no puede exceder los 450 caracteres.")]
        public string IdUsuario { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class AgregarFavoritoDto
    {
        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public string IdUsuario { get; set; }
    }
}

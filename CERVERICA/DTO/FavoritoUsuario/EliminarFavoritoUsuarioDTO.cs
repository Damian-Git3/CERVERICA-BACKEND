using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class EliminarFavoritoUsuarioDTO
    {
        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }
    }
}

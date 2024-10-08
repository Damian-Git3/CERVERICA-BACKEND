using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.Comentarios
{
    public class NuevoComentario
    {
        [Required(ErrorMessage = "La puntuación es obligatoria.")]
        [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
        public int Puntuacion { get; set; }

        [Required(ErrorMessage = "El texto del comentario es obligatorio.")]
        [StringLength(500, ErrorMessage = "El comentario no puede exceder los 500 caracteres.")]
        public string TextoComentario { get; set; }

        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }
    }
}

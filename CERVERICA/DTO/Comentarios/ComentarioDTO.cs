using System;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.Comentarios
{
    public class ComentarioDTO
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La puntuación es obligatoria.")]
        [Range(1, 5, ErrorMessage = "La puntuación debe estar entre 1 y 5.")]
        public float Puntuacion { get; set; }

        [Required(ErrorMessage = "El texto del comentario es obligatorio.")]
        [StringLength(500, ErrorMessage = "El comentario no puede exceder los 500 caracteres.")]
        public string TextoComentario { get; set; }

        [Required(ErrorMessage = "El nombre de usuario es obligatorio.")]
        public string NombreUsuario { get; set; }

        public DateTime Fecha { get; set; }

        [Required(ErrorMessage = "El ID de usuario es obligatorio.")]
        public string IdUsuario { get; set; }

        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }
    }
}

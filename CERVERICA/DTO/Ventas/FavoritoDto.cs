using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Dtos
{
    public class FavoritoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public string IdUsuario { get; set; }

        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }

        [StringLength(500, ErrorMessage = "Las especificaciones no pueden exceder los 500 caracteres.")]
        public string Especificaciones { get; set; }

        [StringLength(500, ErrorMessage = "La descripción no puede exceder los 500 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La imagen es obligatoria.")]
        public string Imagen { get; set; }

        [StringLength(255, ErrorMessage = "La ruta del fondo no puede exceder los 255 caracteres.")]
        public string RutaFondo { get; set; }
    }
}

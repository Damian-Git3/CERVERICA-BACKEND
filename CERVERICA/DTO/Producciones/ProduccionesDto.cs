using System.ComponentModel.DataAnnotations;
using CERVERICA.Models;

namespace CERVERICA.Dtos
{
    public class ProduccionesDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La fecha de producción es obligatoria.")]
        public DateTime FechaProduccion { get; set; }

        [Required(ErrorMessage = "La fecha del próximo paso es obligatoria.")]
        public DateTime FechaProximoPaso { get; set; }

        [StringLength(500, ErrorMessage = "El mensaje no puede exceder los 500 caracteres.")]
        public string Mensaje { get; set; }

        [Required(ErrorMessage = "El estatus es obligatorio.")]
        public int Estatus { get; set; }

        [Required(ErrorMessage = "El número de tandas es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El número de tandas debe ser mayor o igual a 1.")]
        public int NumeroTandas { get; set; }

        [Required(ErrorMessage = "La fecha de solicitud es obligatoria.")]
        public DateTime FechaSolicitud { get; set; }

        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }

        public RecetasDto Receta { get; set; }

        [StringLength(100, ErrorMessage = "El nombre de la receta no puede exceder los 100 caracteres.")]
        public string NombreReceta { get; set; }

        [Required(ErrorMessage = "El paso es obligatorio.")]
        public int Paso { get; set; }

        [StringLength(200, ErrorMessage = "La descripción del paso no puede exceder los 200 caracteres.")]
        public string DescripcionPaso { get; set; }

        [Required(ErrorMessage = "El ID del usuario que solicita es obligatorio.")]
        public string IdUsuarioSolicitud { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public string IdUsuario { get; set; }

        [StringLength(100, ErrorMessage = "El nombre del usuario no puede exceder los 100 caracteres.")]
        public string NombreUsuario { get; set; }
    }
}

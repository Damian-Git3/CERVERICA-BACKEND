using System.ComponentModel.DataAnnotations;
using CERVERICA.Dtos;
using CERVERICA.Models;

namespace CERVERICA.Dtos
{
    public class ProduccionDto
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

        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }

        [Required(ErrorMessage = "La fecha de solicitud es obligatoria.")]
        public DateTime FechaSolicitud { get; set; }

        [Required(ErrorMessage = "El ID del usuario que solicita es obligatorio.")]
        public string IdUsuarioSolicitud { get; set; }

        [Required(ErrorMessage = "El ID del usuario de producción es obligatorio.")]
        public string IdUsuarioProduccion { get; set; }

        [Required(ErrorMessage = "El paso actual es obligatorio.")]
        public int PasoActual { get; set; }

        [StringLength(200, ErrorMessage = "La descripción del paso actual no puede exceder los 200 caracteres.")]
        public string DescripcionPasoActual { get; set; }

        public List<PasosRecetaDto> PasosReceta { get; set; }

        public RecetaProduccionDto Receta { get; set; }

        public List<ProduccionLoteInsumoDto> ProduccionLoteInsumos { get; set; }
    }
}

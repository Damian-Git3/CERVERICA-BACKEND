using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class AvanzarPasoProduccionDto
    {
        [Required(ErrorMessage = "El ID de producción es obligatorio.")]
        public int IdProduccion { get; set; }

        public string? Mensaje { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "La merma en litros debe ser mayor o igual a 0.")]
        public float? MermaLitros { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class FinalizarProduccionDto
    {
        [Required(ErrorMessage = "Los litros finales son obligatorios.")]
        [Range(0, float.MaxValue, ErrorMessage = "Los litros finales deben ser mayores o iguales a 0.")]
        public float? LitrosFinales { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "La merma en litros debe ser mayor o igual a 0.")]
        public float? MermaLitros { get; set; } = 0;

        [Required(ErrorMessage = "El tipo de envase es obligatorio.")]
        [StringLength(100, ErrorMessage = "El tipo de envase no puede exceder los 100 caracteres.")]
        public string? TipoEnvase { get; set; } = "Botella";

        [Range(1, int.MaxValue, ErrorMessage = "La medida del envase debe ser mayor que 0.")]
        public int? MedidaEnvase { get; set; } = 355;

        [StringLength(500, ErrorMessage = "El mensaje no puede exceder los 500 caracteres.")]
        public string? Mensaje { get; set; } = "";

        public bool? CalcularMerma { get; set; }
        public bool? ProduccionFallida { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class DetalleVentaDto
    {
        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad debe ser al menos 1.")]
        public int Cantidad { get; set; }

        [Required(ErrorMessage = "El paquete es obligatorio.")]
        [Range(1, int.MaxValue, ErrorMessage = "El paquete debe ser al menos 1.")]
        public int Pack { get; set; }

        public float MontoVenta { get; set; }

        [StringLength(50, ErrorMessage = "El tipo de envase no puede exceder los 50 caracteres.")]
        public string? TipoEnvase { get; set; } = "Botella";

        [Range(1, int.MaxValue, ErrorMessage = "La medida del envase debe ser mayor que 0.")]
        public int? MedidaEnvase { get; set; } = 355;
    }
}

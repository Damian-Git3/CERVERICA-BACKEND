using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class VentasDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public string IdUsuario { get; set; }

        [Required(ErrorMessage = "La fecha de venta es obligatoria.")]
        public DateTime FechaVenta { get; set; }

        [Required(ErrorMessage = "El total es obligatorio.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "El total debe ser mayor que cero.")]
        public float Total { get; set; }
    }
}

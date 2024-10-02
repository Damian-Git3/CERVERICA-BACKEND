using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class PasosInsertDto
    {
        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "El orden debe ser mayor que cero.")]
        public int Orden { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "El tiempo debe ser un valor positivo.")]
        public double Tiempo { get; set; }
    }
}

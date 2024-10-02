using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class InsumoInsertDto
    {
        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(45, ErrorMessage = "El nombre no puede exceder los 45 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(450, ErrorMessage = "La descripción no puede exceder los 450 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        public string UnidadMedida { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "La cantidad máxima debe ser un valor positivo.")]
        public float? CantidadMaxima { get; set; } = 0;

        [Range(0, float.MaxValue, ErrorMessage = "La cantidad mínima debe ser un valor positivo.")]
        public float? CantidadMinima { get; set; } = 0;

        [Range(0, float.MaxValue, ErrorMessage = "La merma debe ser un valor positivo.")]
        public float? Merma { get; set; } = 0;
    }
}

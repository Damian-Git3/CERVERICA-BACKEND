using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class InsumosDto
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(45, ErrorMessage = "El nombre no puede exceder los 45 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        [StringLength(450, ErrorMessage = "La descripción no puede exceder los 450 caracteres.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        public string UnidadMedida { get; set; }

        [Required(ErrorMessage = "La cantidad máxima es obligatoria.")]
        [Range(0, float.MaxValue, ErrorMessage = "La cantidad máxima debe ser un valor positivo.")]
        public float CantidadMaxima { get; set; }

        [Required(ErrorMessage = "La cantidad mínima es obligatoria.")]
        [Range(0, float.MaxValue, ErrorMessage = "La cantidad mínima debe ser un valor positivo.")]
        public float CantidadMinima { get; set; }

        [Required(ErrorMessage = "La merma es obligatoria.")]
        [Range(0, float.MaxValue, ErrorMessage = "La merma debe ser un valor positivo.")]
        public float Merma { get; set; }

        [Required(ErrorMessage = "El estado activo es obligatorio.")]
        public bool Activo { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El costo unitario debe ser un valor positivo.")]
        public float? CostoUnitario { get; set; }
    }
}

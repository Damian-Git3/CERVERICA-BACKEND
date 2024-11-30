using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class InsumosCantidadDto
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        public string UnidadMedida { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "La cantidad máxima debe ser un valor positivo.")]
        public float CantidadMaxima { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "La cantidad mínima debe ser un valor positivo.")]
        public float CantidadMinima { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El costo unitario debe ser un valor positivo.")]
        public float CostoUnitario { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "La merma debe ser un valor positivo.")]
        public float Merma { get; set; }

        public bool Activo { get; set; }
        public bool Fijo { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "La cantidad total de lotes debe ser un valor positivo.")]
        public float CantidadTotalLotes { get; set; }
    }
}

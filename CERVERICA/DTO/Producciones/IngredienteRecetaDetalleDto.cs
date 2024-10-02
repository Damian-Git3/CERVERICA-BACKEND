using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class IngredienteRecetaDetalleDto
    {
        [Required(ErrorMessage = "El ID del insumo es obligatorio.")]
        public int IdInsumo { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0, float.MaxValue, ErrorMessage = "La cantidad debe ser mayor o igual a 0.")]
        public float Cantidad { get; set; }

        [Required(ErrorMessage = "El nombre del insumo es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre del insumo no puede exceder los 100 caracteres.")]
        public string NombreInsumo { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        [StringLength(50, ErrorMessage = "La unidad de medida no puede exceder los 50 caracteres.")]
        public string UnidadMedida { get; set; }

        [Required(ErrorMessage = "El costo unitario es obligatorio.")]
        [Range(0, float.MaxValue, ErrorMessage = "El costo unitario debe ser mayor o igual a 0.")]
        public float CostoUnitario { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class IngredienteRecetaDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        public float Cantidad { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La unidad de medida es obligatoria.")]
        public string UnidadMedida { get; set; }
    }
}

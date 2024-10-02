using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class IngredienteRecetaInsertDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        public float Cantidad { get; set; }
    }
}

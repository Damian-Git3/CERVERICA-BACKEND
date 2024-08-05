using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class IngredienteRecetaDto
    {
        [Required]
        public int IdInsumo { get; set; }

        [Required]
        public float Cantidad { get; set; }
    }
}

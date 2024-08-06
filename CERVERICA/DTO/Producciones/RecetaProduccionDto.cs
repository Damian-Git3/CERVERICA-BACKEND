using CERVERICA.Dtos;

namespace CERVERICA.Dtos
{
    public class RecetaProduccionDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public List<IngredienteRecetaDetalleDto> IngredientesReceta { get; set; }
    }
}

using CERVERICA.Models;

namespace CERVERICA.DTO.Stock
{
    public class StockDTO
    {
        public int Id { get; set; }
        public int IdReceta { get; set; }
        public Receta Receta { get; set; }
    }
}

using CERVERICA.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Dtos
{
    public class PedidoMayoristaInsertDto
    {
        public int IdMayorista { get; set; }
        public int PlazoMeses { get; set; }

        public string Observaciones { get; set; }

        public List<ProductoPedidoInsertDto> ListaCervezas { get; set; }
    }

    public class ProductoPedidoInsertDto
    {
        public int IdReceta { get; set; }

        public int Cantidad { get; set; }
    }
}

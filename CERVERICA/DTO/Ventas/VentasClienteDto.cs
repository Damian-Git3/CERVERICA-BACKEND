using CERVERICA.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.Ventas
{
    public class VentasClienteDto
    {

        public DateTime FechaVenta { get; set; }

        public float Total { get; set; }

        public int TipoVenta { get; set; }
    }
}

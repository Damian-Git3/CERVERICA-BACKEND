using CERVERICA.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class VentasClienteDto
    {

        public DateTime FechaVenta { get; set; }

        public float Total { get; set; }

        public MetodoPago MetodoPago { get; set; }

        public MetodoEnvio MetodoEnvio { get; set; }
        
        public EstatusVenta EstatusVenta { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class ReglaPuntos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Monto mínimo necesario para empezar a otorgar puntos
        public decimal MontoMinimo { get; set; }

        // Número de puntos otorgados por cada unidad monetaria (por ejemplo, 1 punto por cada $10)
        public int PuntosPorMonto { get; set; }
    }
}
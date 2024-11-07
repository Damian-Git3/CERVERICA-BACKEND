using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class ConfiguracionVentasMayoreo
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public int PlazoMaximoPago { get; set; }

        [Required]
        public bool PagosMensuales { get; set; }

        [Required]
        public float MontoMinimoMayorista { get; set; }

        [Required]
        public DateTime FechaModificacion { get; set; }
    }
}

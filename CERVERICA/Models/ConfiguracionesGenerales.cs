using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class ConfiguracionesGenerales
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public float MinimoCompraEnvioGratis { get; set; }

        [Required]
        public bool PromocionesAutomaticas { get; set; }

        [Required]
        public bool NotificacionPromocionesWhatsApp { get; set; }

        [Required]
        public bool NotificacionPromocionesEmail { get; set; }

        [Required]
        public int TiempoRecordatorioCarritoAbandonado { get; set; }
        [Required]
        public int TiempoRecordatorioRecomendacionUltimaCompra { get; set; }

        [Required]
        public DateTime FechaModificacion { get; set; }

        [Required]
        public int FrecuenciaReclasificacionClientes { get; set; }

        [Required]
        public int FrecuenciaMinimaMensualClienteFrecuente { get; set; }

        [Required]
        public int TiempoSinComprasClienteInactivo { get; set; }
    }
}

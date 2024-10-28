using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class SolicitudesCambioAgente
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(AgenteVentaActual))]
        public string? IdAgenteVentaActual { get; set; }
        public virtual ApplicationUser? AgenteVentaActual { get; set; }

        [ForeignKey(nameof(AgenteVentaNuevo))]
        public string? IdAgenteVentaNuevo { get; set; }
        public virtual ApplicationUser? AgenteVentaNuevo { get; set; }

        [Required]
        public DateTime FechaSolicitud { get; set; }

        [Required]
        public string Motivo { get; set; }

        [Required]
        public int Solicitante { get; set; } // 1 cliente 2 agente 3 admin

        [ForeignKey(nameof(Mayorista))]
        public string? IdMayorista { get; set; }
        public virtual ClienteMayorista? Mayorista { get; set; }

    }
}

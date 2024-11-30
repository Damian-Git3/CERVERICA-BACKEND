using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CERVERICA.Models
{
    public class SolicitudMayorista
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaCierre { get; set; }
        public EstatusSolicitudMayorista Estatus { get; set; }
        public string? mensajeRechazo { get; set; }
        [ForeignKey(nameof(Mayorista))]
        public int IdMayorista { get; set; }
        public virtual ClienteMayorista Mayorista { get; set; }

        [ForeignKey(nameof(Agente))]
        public String? IdAgente { get; set; }
        [JsonIgnore]
        public virtual ApplicationUser? Agente { get; set; }
    }

    public enum EstatusSolicitudMayorista
    {
        NuevoPedido = 1,
        Confirmando = 2,
        Concretado = 3,
        Cancelado = 4,
        Rechazado = 5,
    }
}

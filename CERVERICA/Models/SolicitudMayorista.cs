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

        [Required]
        public EstatusSolicitud Estatus { get; set; }

        [ForeignKey("Mayorista")]
        public string IdMayorista { get; set; }

        [ForeignKey("Usuario")]
        public int IdAgenteVenta { get; set; }

        [JsonIgnore]
        public virtual ClienteMayorista? Mayorista { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser? Usuario { get; set; }
        public enum EstatusSolicitud
        {
            Prospecto = 1,
            NuevoPedido = 2,
            Contactado = 3,
            Cerrado = 4,
            Cancelado = 5,
        }
    }
}

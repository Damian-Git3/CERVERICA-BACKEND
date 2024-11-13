using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CERVERICA.Models
{
   public class ClienteMayorista
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [Required]
        public string RFCEmpresa { get; set; }
        [Required]
        public string NombreEmpresa { get; set; }
        [Required]
        public string DireccionEmpresa { get; set; }
        [Required]
        public string TelefonoEmpresa { get; set; }
        [Required]
        public string EmailEmpresa { get; set; }
        [Required]
        public string NombreContacto { get; set; }
        [Required]
        public string CargoContacto { get; set; }
        [Required]
        public string TelefonoContacto { get; set; }
        [Required]
        public string EmailContacto { get; set; }


        // Relación uno a uno con el usuario ClienteMayorista (ApplicationUser)
        [ForeignKey("Usuario")]
        public string IdUsuario { get; set; }
        [JsonIgnore]
        public ApplicationUser Usuario { get; set; }

        // Nueva relación uno a uno con el usuario AgenteVenta (ApplicationUser)
        [ForeignKey("AgenteVenta")]
        public string? IdAgenteVenta { get; set; }
        [JsonIgnore]
        public ApplicationUser? AgenteVenta { get; set; }

        // Relación uno a muchos con la tabla SolicitudesCambioAgente
        public ICollection<SolicitudesCambioAgente> SolicitudesCambioAgente { get; set; }

        // Relación uno a muchos con la tabla PedidoMayoreo
        [JsonIgnore]
        public ICollection<PedidoMayoreo> PedidosMayoreo { get; set; }

        [JsonIgnore]
        public ICollection<Pago> Pagos { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

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
        public string RFCEmpresa { get; set; }
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
        public ApplicationUser Usuario { get; set; }

        // Nueva relación uno a uno con el usuario AgenteVenta (ApplicationUser)
        [ForeignKey("AgenteVenta")]
        public string IdAgenteVenta { get; set; }
        public ApplicationUser AgenteVenta { get; set; }

        // Relación uno a muchos con la tabla SolicitudesCambioAgente
        public ICollection<SolicitudesCambioAgente> SolicitudesCambioAgente { get; set; }

        // Relación uno a muchos con la tabla SolicitudesPedidoMayoreo
        public ICollection<SolicitudesPedidoMayoreo> SolicitudesPedidoMayoreo { get; set; }
    }
}

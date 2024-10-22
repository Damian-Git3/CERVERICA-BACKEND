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
        [ForeignKey("User")]
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        // Nueva relación uno a uno con el usuario AgenteVenta (ApplicationUser)
        [ForeignKey("AgenteVenta")]
        public string AgenteVentaId { get; set; }
        public ApplicationUser AgenteVenta { get; set; }
    }
}

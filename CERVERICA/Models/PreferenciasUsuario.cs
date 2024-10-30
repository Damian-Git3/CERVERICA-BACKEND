using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class PreferenciasUsuario
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Usuario))]
        public string IdUsuario { get; set; }

        [Required]
        public bool NotificacionesEmail { get; set; }

        [Required]
        public bool NotificacionesWhatsapp { get; set; }

        [Required]
        public bool RecibePromocionesEmail { get; set; }

        [Required]
        public bool RecibePromocionesWhatsapp { get; set; }

        public virtual ApplicationUser? Usuario { get; set; }
    }
}

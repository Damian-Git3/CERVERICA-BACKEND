using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Models
{
    public class InteraccionComprador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdInteraccion { get; set; }

        [Required]
        public string IdUsuario { get; set; }  // Relación con ApplicationUser
        public ApplicationUser Usuario { get; set; }

        public DateTime FechaInteraccion { get; set; }
        public string TipoInteraccion { get; set; }  // Email, Teléfono, Chat
        public string Descripcion { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Models
{
    public class PreferenciasComprador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPreferencia { get; set; }

        [Required]
        public string IdUsuario { get; set; }  // Relación con ApplicationUser
        public ApplicationUser Usuario { get; set; }

        public bool RecibePromociones { get; set; }
        public string EstiloCervezaFavorito { get; set; }
        public string Descripcion { get; set; }
    }
}

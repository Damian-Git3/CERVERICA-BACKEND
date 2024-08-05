using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class LogLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public string IdUsuario { get; set; }
        public ApplicationUser Usuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public Boolean Exitoso { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    [Table("logs_login", Schema = "cerverica")]
    public class LogLogin
    {
        [Key]
        public int Id { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public string IdUsuario { get; set; }
        public ApplicationUser Usuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }
    }
}

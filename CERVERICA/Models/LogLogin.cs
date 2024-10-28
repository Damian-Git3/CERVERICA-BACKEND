using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class LogLogin
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Usuario))]
        public string IdUsuario { get; set; }
        public virtual ApplicationUser? Usuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public Boolean Exitoso { get; set; }
    }
}

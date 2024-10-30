using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class Carrito
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Usuario))]
        public string IdUsuario { get; set; }

        [Required]
        public DateTime FechaModificacion { get; set; }

        public virtual ApplicationUser? Usuario { get; set; }

    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

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

        [JsonIgnore]
        public virtual ApplicationUser? Usuario { get; set; }

    }
}

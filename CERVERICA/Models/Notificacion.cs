using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CERVERICA.Models
{
    public class Notificacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("Usuario")]
        public string IdUsuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int Tipo { get; set; }

        [Required]
        public string Mensaje { get; set; }

        [Required]
        public bool Visto { get; set; }
    }
}

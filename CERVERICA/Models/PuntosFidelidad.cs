
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class PuntosFidelidad
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign key hacia el usuario
        [ForeignKey("Usuario")]
        public string IdUsuario { get; set; }
        public virtual ApplicationUser? Usuario { get; set; }

        public int? PuntosAcumulados { get; set; }
        public int? PuntosRedimidos { get; set; }
        public int? PuntosDisponibles { get; set; }

        public DateTime? FechaUltimaActualizacion { get; set; }
    }
}
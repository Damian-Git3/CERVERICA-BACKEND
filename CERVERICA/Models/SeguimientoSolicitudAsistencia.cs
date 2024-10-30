using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class SeguimientoSolicitudAsistencia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(SolicitudAsistencia))]
        public int IdSolicitudAsistencia { get; set; }
        public virtual SolicitudAsistencia SolicitudAsistencia { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public DateTime FechaSeguimiento { get; set; }

        [Required]
        public string Mensaje { get; set; }
    }
}

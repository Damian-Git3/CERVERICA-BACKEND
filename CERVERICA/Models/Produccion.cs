using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    [Table("producciones", Schema = "cerverica")]
    public class Produccion
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public DateTime FechaProduccion { get; set; }

        public string Mensaje { get; set; }

        [Required]
        public byte Status { get; set; }

        [Required]
        public int Tandas { get; set; }

        public float? LitrosFinales { get; set; }

        [ForeignKey(nameof(Receta))]
        public int IdReceta { get; set; }
        public Receta Receta { get; set; }

        public DateTime? FechaSolicitud { get; set; }

        [ForeignKey(nameof(IdUsuarioSolicitud))]
        public string IdUsuarioSolicitud { get; set; }
        public ApplicationUser UsuarioSolicitud { get; set; }

        [ForeignKey(nameof(IdUsuarioProduccion))]
        public string IdUsuarioProduccion { get; set; }
        public ApplicationUser UsuarioProduccion { get; set; }

        [Required]
        public int Paso { get; set; }

        public float? MermaLitros { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CERVERICA.Models
{
    public class CategoriaAsistencia
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public string Nombre { get; set; }

        [Required]
        public bool Estatus { get; set; }
        [JsonIgnore]
        public ICollection<SolicitudAsistencia> SolicitudesAsistencia { get; set; }
    }
}

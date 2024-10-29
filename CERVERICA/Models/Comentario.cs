using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CERVERICA.Models
{
    public class Comentario
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public float Puntuacion { get; set; }

        [Required]
        [StringLength(500, ErrorMessage = "El comentario no puede exceder los 500 caracteres.")]
        public string TextoComentario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        public int? Paquete { get; set; }

        [ForeignKey(nameof(DetalleVenta))]
        public int IdDetalleVenta { get; set; }
        public virtual DetalleVenta? DetalleVenta { get; set; }

        public bool? Oculto { get; set; }

        public bool? Editado { get; set; }


        [Required]
        [ForeignKey(nameof(Usuario))]
        public string IdUsuario { get; set; }

        [JsonIgnore]
        public virtual ApplicationUser? Usuario { get; set; }

        [Required]
        [ForeignKey("Receta")]
        public int IdReceta { get; set; }

        [JsonIgnore]
        public virtual Receta? Receta { get; set; }
    }
}

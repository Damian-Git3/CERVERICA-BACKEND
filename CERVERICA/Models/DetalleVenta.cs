using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CERVERICA.Models
{
    public class DetalleVenta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Venta))]
        public int IdVenta { get; set; }
        [JsonIgnore]
        public Venta Venta { get; set; }


        [ForeignKey(nameof(Stock))]
        public int IdStock { get; set; }
        [JsonIgnore]
        public Stock Stock { get; set; }

        [ForeignKey(nameof(Receta))]
        public int IdReceta { get; set; }
        [JsonIgnore]
        public virtual Receta? Receta { get; set; }


        public float MontoVenta { get; set; }

        public int? Cantidad { get; set; }

        public int? Pack { get; set; }

        [JsonIgnore]
        public virtual ICollection<Comentario>? Comentarios { get; set; }

    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace CERVERICA.Models
{
    public class ProductoCarrito
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Receta))]
        public int IdReceta { get; set; }

        [ForeignKey(nameof(Carrito))]
        public int IdCarrito { get; set; }

        [Required]
        public int CantidadPaquete { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public DateTime FechaModificacion { get; set; }

        
        public virtual Receta? Receta { get; set; }
        [JsonIgnore]
        public virtual Carrito? Carrito { get; set; }
    }
}

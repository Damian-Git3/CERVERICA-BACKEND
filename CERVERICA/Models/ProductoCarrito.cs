using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Models
{
    public class ProductoCarrito
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Usuario")]
        public string IdUsuario { get; set; }
        [ForeignKey("Receta")]
        public int IdReceta { get; set; }
        public int CantidadLote { get; set; }
        public int Cantidad { get; set; }
        public virtual ApplicationUser Usuario { get; set; }
        public virtual Receta Receta { get; set; }
    }
}

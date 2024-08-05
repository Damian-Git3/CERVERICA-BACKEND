using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class Stock
    {
        [Key]
        public int Id { get; set; }

        public DateTime? FechaEntrada { get; set; }

        [Required]
        public int Cantidad { get; set; }
 
        public string TipoEnvase { get; set; }

        public int MedidaEnvase { get; set; }

        public int? Merma { get; set; }

        [ForeignKey(nameof(Produccion))]
        public int IdProduccion { get; set; }
        public Produccion Produccion { get; set; }

        [ForeignKey(nameof(Receta))]
        public int IdReceta { get; set; }
        public Receta Receta { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public string IdUsuario { get; set; }
        public ApplicationUser Usuario { get; set; }
    }
}

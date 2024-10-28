using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Models
{
    public class PasosReceta 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Receta))]
        public int IdReceta { get; set; }
        public Receta Receta { get; set; }

        [Required]
        [StringLength(2500)] 
        public string Descripcion { get; set; }

        [Required]
        public int Orden { get; set; }

        [Required]
        public double Tiempo { get; set; }
    }
}

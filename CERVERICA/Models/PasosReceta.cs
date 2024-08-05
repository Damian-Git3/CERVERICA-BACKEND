using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Models
{
    public class PasosReceta 
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int RecetaId { get; set; }
        public Receta Receta { get; set; }

        [Required]
        [StringLength(500)] // Establecer una longitud máxima razonable
        public string Descripcion { get; set; }

        [Required]
        public int Orden { get; set; }

        [Required]
        public int Tiempo { get; set; } // Asumir que el tiempo está en minutos, por ejemplo
    }
}

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Models
{
    public class HistorialPrecios
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey(nameof(Receta))]
        public required int IdReceta { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public required double Paquete1 { get; set; }

        [Required]
        public required double Paquete6 { get; set; }

        [Required]
        public required double Paquete12 { get; set; }

        [Required]
        public required double Paquete24 { get; set; }

        [Required]
        public required double CostoProduccionUnidad { get; set; }

        [Required]
        public required double PrecioUnitarioMinimoMayoreo { get; set; }

        [Required]
        public required double PrecioUnitarioBaseMayoreo { get; set; }

        // Propiedad de navegación
        public Receta Receta { get; set; }

        // Constructor
        public HistorialPrecios()
        {
            Fecha = DateTime.Now; // Valor por defecto
            Paquete1 = 0.0; // Valor por defecto
            Paquete6 = 0.0; // Valor por defecto
            Paquete12 = 0.0; // Valor por defecto
            Paquete24 = 0.0; // Valor por defecto
            CostoProduccionUnidad = 0.0; // Valor por defecto
            PrecioUnitarioMinimoMayoreo = 0.0; // Valor por defecto
            PrecioUnitarioBaseMayoreo = 0.0; // Valor por defecto
        }

    }
}

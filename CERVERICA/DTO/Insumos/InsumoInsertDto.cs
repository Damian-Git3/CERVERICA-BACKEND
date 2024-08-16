using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class InsumoInsertDto
    {
        [Required]
        [StringLength(45)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(450)]
        public string Descripcion { get; set; }

        [Required]
        public string UnidadMedida { get; set; }

        public float? CantidadMaxima { get; set; } = 0;

        public float? CantidadMinima { get; set; } = 0;

        public float? Merma { get; set; } = 0;
    }
}

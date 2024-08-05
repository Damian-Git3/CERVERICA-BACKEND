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

        [Required]
        public float CantidadMaxima { get; set; }

        [Required]
        public float CantidadMinima { get; set; }

        [Required]
        public float Merma { get; set; }
    }
}

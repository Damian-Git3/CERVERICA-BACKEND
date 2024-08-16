using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class RecetasDto
    {
        public int Id { get; set; }
        [Required]
        public float LitrosEstimados { get; set; }

        public float? PrecioLitro { get; set; }

        public string Descripcion { get; set; }
        public float? PrecioPaquete1 { get; set; }
        public float? PrecioPaquete6 { get; set; }
        public float? PrecioPaquete12 { get; set; }
        public float? PrecioPaquete24 { get; set; }


        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        public float CostoProduccion { get; set; }

        [Required]
        public string Imagen { get; set; }

        public string RutaFondo { get; set; }

        public DateTime FechaRegistrado { get; set; }

        [Required]
        public bool Activo { get; set; }
    }
}

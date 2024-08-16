using CERVERICA.Models;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class RecetaDetallesDto
    {
        public int Id { get; set; }
        [Required]
        public float LitrosEstimados { get; set; }

        public float? PrecioLitro { get; set; }

        public string Especificaciones { get; set; }

        public float? PrecioPaquete1 { get; set; }
        public float? PrecioPaquete6 { get; set; }
        public float? PrecioPaquete12 { get; set; }
        public float? PrecioPaquete24 { get; set; }
        public string Descripcion { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        public float CostoProduccion { get; set; }

        [Required]
        public string Imagen { get; set; }
        public string RutaFondo { get; set; }

        [Required]
        public bool Activo { get; set; }

        public ICollection<IngredienteRecetaDto> IngredientesReceta { get; set; }
        public ICollection<PasosRecetaDto>? PasosReceta { get; set; }
    }
}

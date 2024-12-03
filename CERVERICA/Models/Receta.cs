using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Models
{
    public class Receta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public float LitrosEstimados { get; set; }

        public float? PrecioUnitarioMinimoMayoreo { get; set; }

        public float? PrecioUnitarioBaseMayoreo { get; set; }

        public float? PorcentajeDescuento { get; set; }

        public float PrecioLitro { get; set; }
        public float PrecioPaquete1 { get; set; }
        public float PrecioPaquete6 { get; set; }
        public float PrecioPaquete12 { get; set; }
        public float PrecioPaquete24 { get; set; }
        public int? LotesMinimos { get; set; }
        public int? LotesMaximos { get; set; }

        public float? Puntuacion { get; set; }

        public string? Especificaciones { get; set; }
        public string? Descripcion { get; set; }

        [Required]
        [StringLength(100)]
        public required string Nombre { get; set; }

        [Required]
        public float CostoProduccion { get; set; }

        [Required]
        public string? Imagen { get; set; }
        public string? RutaFondo { get; set; }

        [Required]
        public float TiempoVida { get; set; }

        [Required]
        public bool Activo { get; set; }

        [Required]
        public bool AptaVentaMayorista { get; set; }

        [Column(TypeName = "Date")]
        public DateTime FechaRegistrado { get; set; }

        public ICollection<Comentario>? Comentarios { get; set; }

        public ICollection<IngredienteReceta>? IngredientesReceta { get; set; }

        public ICollection<Produccion>? Producciones { get; set; }

        public ICollection<Stock>? Stocks { get; set; }

        public ICollection<PasosReceta>? PasosReceta { get; set; }

        public ICollection<FavoritosComprador>? FavoritosComprador { get; set; }

        public ICollection<RecetaCategoriaCerveza>? RecetaCategoriaCerveza { get; set; }

        public ICollection<Cupones>? Cupones { get; set; }

        public ICollection<HistorialPrecios>? HistorialPrecios { get; set; }
    }
}

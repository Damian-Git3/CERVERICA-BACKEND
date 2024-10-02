using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class RecetaConStockDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La descripción es obligatoria.")]
        public string Descripcion { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "Los litros estimados deben ser un valor positivo.")]
        public float LitrosEstimados { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio por litro debe ser un valor positivo.")]
        public float? PrecioLitro { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 1 debe ser un valor positivo.")]
        public float? PrecioPaquete1 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 6 debe ser un valor positivo.")]
        public float? PrecioPaquete6 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 12 debe ser un valor positivo.")]
        public float? PrecioPaquete12 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 24 debe ser un valor positivo.")]
        public float? PrecioPaquete24 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El costo de producción debe ser un valor positivo.")]
        public float CostoProduccion { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "La cantidad en stock debe ser un valor positivo.")]
        public float CantidadEnStock { get; set; }
    }
}

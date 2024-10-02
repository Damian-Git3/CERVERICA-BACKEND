using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class UpdateRecetaPrecioDto
    {
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
    }
}

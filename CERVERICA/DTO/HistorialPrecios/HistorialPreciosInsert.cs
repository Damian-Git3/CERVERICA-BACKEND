using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class HistorialPreciosInsert
    {
        [Required(ErrorMessage = "IdReceta Obligatorio")]
        [Range(1, int.MaxValue, ErrorMessage = "IdReceta debe ser mayor a 0")]
        public int IdReceta { get; set; }

        [Required(ErrorMessage = "Precio de Paquete1 Obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Precio de Paquete1 debe ser mayor a 0")]
        public double Paquete1 { get; set; }

        [Required(ErrorMessage = "Precio de Paquete6 Obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Precio de Paquete6 debe ser mayor a 0")]
        public double Paquete6 { get; set; }

        [Required(ErrorMessage = "Precio de Paquete12 Obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Precio de Paquete12 debe ser mayor a 0")]
        public double Paquete12 { get; set; }

        [Required(ErrorMessage = "Precio de Paquete24 Obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Precio de Paquete24 debe ser mayor a 0")]
        public double Paquete24 { get; set; }

        [Required(ErrorMessage = "Costo de Produccion por Unidad Obligatorio")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Costo de Produccion por Unidad debe ser mayor a 0")]
        public double CostoProduccionUnidad { get; set; }

        [Required(ErrorMessage = "Precio Unitario Minimo de Mayoreo Obligatorio")]
        public double PrecioUnitarioMinimoMayoreo { get; set; }

        [Required(ErrorMessage = "Precio Unitario Base de Mayoreo Obligatorio")]
        public double PrecioUnitarioBaseMayoreo { get; set; }
    }
}

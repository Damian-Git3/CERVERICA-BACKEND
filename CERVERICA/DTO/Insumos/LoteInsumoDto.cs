using CERVERICA.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class LoteInsumoDto
    {
        public int Id { get; set; }

        [Required]
        public int IdProveedor { get; set; }

        [Required]
        public int IdInsumo { get; set; }

        [Required]
        public string IdUsuario { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaCaducidad { get; set; }

        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public float Cantidad { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FechaCompra { get; set; }

        [Required]
        public float Merma { get; set; }

        [Required]
        public float? Caducado { get; set; }

        [Required]
        public float PrecioUnidad { get; set; }

        [Required]

        [Range(0, float.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0")]
        public float MontoCompra { get; set; }
    }
}

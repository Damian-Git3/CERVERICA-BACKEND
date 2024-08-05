using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class LoteInsumoInsertDto
    {
        [Required]
        public int IdProveedor { get; set; }

        [Required]
        public int IdInsumo { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaCaducidad { get; set; }

        [Required]
        public float Cantidad { get; set; }

        [Required]
        public float MontoCompra { get; set; }
    }
}

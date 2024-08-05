using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class LoteInsumoEditarDto
    {
        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaCaducidad { get; set; }

        [Required]
        public float Cantidad { get; set; }

        [Required]
        public float MontoCompra { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class LoteInsumoEditarDto
    {
        [Required(ErrorMessage = "El ID del proveedor es obligatorio.")]
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "El ID del insumo es obligatorio.")]
        public int IdInsumo { get; set; }

        [Required(ErrorMessage = "La fecha de caducidad es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaCaducidad { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        public float Cantidad { get; set; }

        [Required(ErrorMessage = "El monto de compra es obligatorio.")]
        public float MontoCompra { get; set; }
    }
}

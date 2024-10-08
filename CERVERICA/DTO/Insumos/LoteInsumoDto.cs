using CERVERICA.Models;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CERVERICA.Dtos
{
    public class LoteInsumoDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "El ID del proveedor es obligatorio.")]
        public int IdProveedor { get; set; }

        [Required(ErrorMessage = "El ID del insumo es obligatorio.")]
        public int IdInsumo { get; set; }

        [Required(ErrorMessage = "El ID del usuario es obligatorio.")]
        public string IdUsuario { get; set; }

        [Required(ErrorMessage = "La fecha de caducidad es obligatoria.")]
        [DataType(DataType.Date)]
        public DateTime FechaCaducidad { get; set; }

        [Required(ErrorMessage = "La cantidad es obligatoria.")]
        [Range(0, float.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public float Cantidad { get; set; }

        [Required(ErrorMessage = "La fecha de compra es obligatoria.")]
        [DataType(DataType.DateTime)]
        public DateTime FechaCompra { get; set; }

        [Required(ErrorMessage = "La merma es obligatoria.")]
        public float Merma { get; set; }

        [Required(ErrorMessage = "El estado de caducado es obligatorio.")]
        public float? Caducado { get; set; } = 0;

        [Required(ErrorMessage = "El precio por unidad es obligatorio.")]
        public float PrecioUnidad { get; set; }

        public int? NumeroProducciones { get; set; }

        [Required(ErrorMessage = "El monto de compra es obligatorio.")]
        [Range(0, float.MaxValue, ErrorMessage = "La cantidad debe ser mayor a 0.")]
        public float MontoCompra { get; set; }

        public Proveedor? Proveedor { get; set; }
        public Insumo? Insumo { get; set; }
        public ApplicationUser? Usuario { get; set; }
    }
}

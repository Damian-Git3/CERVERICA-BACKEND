using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.OpenApi.Services;

namespace CERVERICA.Models
{
    public class LoteInsumo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Proveedor))]
        public int IdProveedor { get; set; }

        [ForeignKey(nameof(Insumo))]
        public int IdInsumo { get; set; }

        [ForeignKey(nameof(Usuario))]
        public string IdUsuario { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime FechaCaducidad { get; set; }

        [Required]
        public float Cantidad { get; set; }

        [Required]
        [DataType(DataType.DateTime)]
        public DateTime FechaCompra { get; set; }

        [Required]
        public float PrecioUnidad { get; set; }

        [Required]
        public float MontoCompra { get; set; }

        [Required]
        public float Merma { get; set; }

        [Required]
        public float? Caducado { get; set; } = 0;

        public Proveedor Proveedor { get; set; }
        public Insumo Insumo { get; set; }
        public ApplicationUser Usuario { get; set; }
        public ICollection<ProduccionLoteInsumo> ProduccionLoteInsumos { get; set; }
    }
}

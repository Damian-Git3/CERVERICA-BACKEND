using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    [Table("proveedores", Schema = "cerverica")]
    public class Proveedor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(45)]
        public string Empresa { get; set; }

        [Required]
        [StringLength(45)]
        public string Direccion { get; set; }

        [Required]
        [StringLength(45)]
        public string NombreProveedor { get; set; }

        public ICollection<LoteInsumo> LotesInsumos { get; set; }
    }
}

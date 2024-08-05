using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class Proveedor
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Empresa { get; set; }

        [Required]
        [Phone]
        public string Telefono { get; set; }

        [Required]
        [StringLength(45)]
        public string Direccion { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(45)]
        public string NombreContacto { get; set; }

        [Required]
        public bool Activo { get; set; }

        public ICollection<LoteInsumo> LotesInsumos { get; set; }
    }
}

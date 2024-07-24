using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    [Table("clientes", Schema = "cerverica")]
    public class Cliente
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(45)]
        public string Nombre { get; set; }

        [Required]
        [StringLength(45)]
        public string Correo { get; set; }

        [Required]
        [StringLength(45)]
        public string Telefono { get; set; }

        [Required]
        [StringLength(45)]
        public string Direccion { get; set; }
    }
}

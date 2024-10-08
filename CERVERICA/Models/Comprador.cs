using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Models
{
    public class Comprador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int IdPerfil { get; set; }

        [Required]
        public string IdUsuario { get; set; }  // Relación con ApplicationUser
        public ApplicationUser Usuario { get; set; }

        public string Direccion { get; set; }
        public string Ciudad { get; set; }
        public string Pais { get; set; }
        public string CodigoPostal { get; set; }

        public DateTime? FechaNacimiento { get; set; }
        public string MetodoContactoPreferido { get; set; }
    }

}

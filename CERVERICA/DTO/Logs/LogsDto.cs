using CERVERICA.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class LogsDto
    {
        public int Id { get; set; }

        [ForeignKey(nameof(IdUsuario))]
        public string IdUsuario { get; set; }
        public ApplicationUser Usuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        [StringLength(255)]
        public string Descripcion { get; set; }
    }
}

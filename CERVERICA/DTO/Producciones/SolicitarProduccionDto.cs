using CERVERICA.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class SolicitarProduccionDto
    {
        [Required]
        public int NumeroTandas { get; set; }

        [Required]
        public int IdReceta { get; set; }

        public string IdUsuario { get; set; }
    }
}

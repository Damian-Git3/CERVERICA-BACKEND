using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.Dtos
{
    public class FavoritoDto
    {
        public int Id { get; set; }

        public string IdUsuario { get; set; }

        public int IdReceta { get; set; }

        public string Especificaciones { get; set; }
        public string Descripcion { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        public string Imagen { get; set; }
        public string RutaFondo { get; set; }
    }
}

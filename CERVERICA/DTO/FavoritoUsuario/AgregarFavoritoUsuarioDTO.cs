using System.ComponentModel.DataAnnotations.Schema;

namespace CERVERICA.DTO.FavoritoUsuario
{
    public class AgregarFavoritoUsuarioDTO
    {
        public int Id { get; set; }
        public string IdUsuario { get; set; }
        public int IdReceta { get; set; }
    }
}

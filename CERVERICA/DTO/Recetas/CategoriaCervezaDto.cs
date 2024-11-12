using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class CategoriaCervezaDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }

        public bool Estatus { get; set; }
    }

    public class CategoriaCervezaPutDto
    {
        public int Id { get; set; }

        public string Nombre { get; set; }
    }
    public class CategoriaCervezaInsertDto
    {

        [Required]
        public string Nombre { get; set; }

    }

    public class AsignarCategoriasRecetaDto
    {
        public int IdReceta { get; set; }

        //arreglo de id de categorias
        public List<CategoriaAsignarDto> CategoriasReceta { get; set; }
    }

    public class CategoriaAsignarDto
    {
        public int Id { get; set; }
    }

}

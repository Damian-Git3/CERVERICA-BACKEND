using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class RecetaCategoriaCerveza
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Receta))]
        public int IdReceta { get; set; }
        public Receta Receta { get; set; }

        [ForeignKey(nameof(CategoriaCerveza))]
        public int IdCategoriaCerveza { get; set; }
        public CategoriaCerveza CategoriaCerveza { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class EstadisticaComprador
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Usuario))]
        public string? IdUsuario { get; set; }
        public virtual ApplicationUser? Usuario { get; set; }

        [Required]
        public int MontoCompras { get; set; }

        [Required]
        public int CantidadCompras { get; set; }

        [Required]
        public int NumeroComentarios { get; set; }

        [Required]
        public float PromedioValoraciones { get; set; }

        [Required]
        public float PromedioValoracionesEditadas { get; set; }

        [Required]
        public int NumeroValoracionesEditadas { get; set; }

        public int AtrasoPagos { get; set; }

        [Required]
        public int FrecuenciaCompras { get; set; }




    }
    //esta declarado en Cupones.cs
    //public enum CategoriaComprador
    //{
    //    Todos = 1,
    //    Frecuente = 2,
    //    Minorista = 3,
    //    Mayorista = 4,
    //    Inactivo = 5
    //}
}

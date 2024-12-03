using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class Cupones
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Usuario))]
        public string? IdUsuario { get; set; }
        public virtual ApplicationUser? Usuario { get; set; }

        [ForeignKey(nameof(Receta))]
        public int? IdReceta { get; set; }
        public virtual Receta? Receta { get; set; }

        [Required]
        public DateTime FechaCreacion { get; set; }

        [Required]
        public DateTime FechaExpiracion { get; set; }

        [Required]
        public string Codigo { get; set; }

        [Required]
        public TipoCupon Tipo { get; set; }

        [Required]
        public int Paquete { get; set; }

        [Required]
        public int Cantidad { get; set; }

        [Required]
        public decimal Valor { get; set; }

        [Required]
        public int Usos { get; set; }

        public decimal MontoMaximo { get; set; }
        public decimal MontoMinimo { get; set; }

        [Required]
        public bool Activo { get; set; }
    }

    public enum TipoCupon
    {
        Porcentaje = 1,
        Fijo = 2
    }
}

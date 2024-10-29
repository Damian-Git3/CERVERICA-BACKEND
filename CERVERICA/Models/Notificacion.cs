using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CERVERICA.Models
{
    public class Notificacion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Usuario))]
        public string IdUsuario { get; set; }
        public virtual ApplicationUser? Usuario { get; set; }

        [Required]
        public DateTime Fecha { get; set; }

        [Required]
        public int Tipo { get; set; }

        [Required]
        public string Mensaje { get; set; }

        [Required]
        public bool Visto { get; set; }

        [Required]
        public Categoria Categoria { get; set; }
    }

    public enum Categoria
    {
        Promocion = 1,
        Informe = 2,
        AtencionAlCliente = 3,
        Empleado = 4
    }

}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class SolicitudesPedidoMayoreo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Mayorista))]
        public string? IdMayorista { get; set; }
        public virtual ClienteMayorista? Mayorista { get; set; }

        public DateTime Fecha { get; set; }

        [ForeignKey(nameof(AgenteVenta))]
        public string? IdAgenteVenta { get; set; }
        public virtual ApplicationUser? AgenteVenta { get; set; }

        public int Estatus { get; set; }
    }
}
//1 solicitado
//2 contactado
//3 cerrado
//4 cancelado
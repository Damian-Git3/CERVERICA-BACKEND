using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class Pago
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }


        [ForeignKey(nameof(Mayorista))]
        public int IdMayorista { get; set; }
        public virtual ClienteMayorista? Mayorista { get; set; }

        [ForeignKey(nameof(PedidoMayoreo))]
        public int IdPedidoMayoreo { get; set; }
        public virtual PedidoMayoreo? PedidoMayoreo { get; set; }

        public string Comprobante { get; set; }

        public DateTime FechaVencimiento { get; set; }

        public DateTime FechaPago { get; set; }

        public float Monto { get; set; }

        public EstatusPago Estatus { get; set; }

        public enum EstatusPago
        {
            Pendiente = 1,
            Pagado = 2,
            Cancelado = 3,
        }
    }
}

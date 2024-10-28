﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class PedidoMayoreo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Venta))]
        public int IdVenta { get; set; }
        public virtual Venta Venta { get; set; }

        [ForeignKey(nameof(ClienteMayorista))]
        public int IdMayorista { get; set; }
        public virtual ClienteMayorista ClienteMayorista { get; set; }

        [ForeignKey(nameof(SolicitudPedidoMayoreo))]
        public int IdSolicitudPedidoMayoreo { get; set; }
        public virtual SolicitudesPedidoMayoreo SolicitudPedidoMayoreo { get; set; }

        [ForeignKey(nameof(AgenteVenta))]
        public string IdAgenteVenta { get; set; }
        public virtual ApplicationUser? AgenteVenta { get; set; }

        public DateTime FechaInicio { get; set; }

        public int NumeroPagos { get; set; }

        public DateTime FechaLimite { get; set; }

        public float MontoTotal { get; set; }  

        public float MontoPorPago { get; set; }

        public string Observaciones { get; set; }
    }
}

using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class SolicitudAsistencia
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Cliente))]
        public string IdCliente { get; set; }
        public virtual ApplicationUser Cliente{ get; set; }

        [ForeignKey(nameof(AgenteVenta))]
        public string IdAgenteVenta { get; set; }
        public virtual ApplicationUser AgenteVenta { get; set; }

        [ForeignKey(nameof(PedidoMayoreo))]
        public int? IdSolicitudMayorista { get; set; }
        public virtual PedidoMayoreo? PedidoMayoreo { get; set; }

        [ForeignKey(nameof(Venta))]
        public int? IdVenta { get; set; }
        public virtual Venta? Venta { get; set; }

        [ForeignKey(nameof(CategoriaAsistencia))]
        public int IdCategoriaAsistencia { get; set; }
        public virtual CategoriaAsistencia CategoriaAsistencia { get; set; }

        [Required]
        public bool Mayoreo { get; set; }

        [Required]
        public string Descripcion { get; set; }

        [Required]
        public DateTime FechaSolicitud { get; set; }

        [Required]
        public DateTime? FechaCierre { get; set; }

        [Required]
        public int Estatus { get; set; }

        [Required]
        public float Valoracion { get; set; }

        [Required]
        public string MensajeValoracion { get; set; }

        [Required]
        public int Tipo { get; set; }

        public ICollection<SeguimientoSolicitudAsistencia> SeguimientosSolicitudAsistencia { get; set; }
        public ICollection<CategoriaAsistencia> CategoriasAsistencia { get; set; }
    }
}

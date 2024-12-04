using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace CERVERICA.Models
{
    public class Venta
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey(nameof(Usuario))]
        public string IdUsuario { get; set; }

        [Required]
        public DateTime FechaVenta { get; set; }

        [Required]
        public float Total { get; set; }
        public int TotalCervezas { get; set; }

        [Required]
        public bool Mayoreo { get; set; } = false;

        [Required]
        public bool Recordatorio { get; set; } = false;

        [Required]
        public MetodoPago MetodoPago { get; set; }

        [Required]
        public MetodoEnvio MetodoEnvio { get; set; }
        [Required]
        public EstatusVenta EstatusVenta { get; set; }
        public int? CuponId { get; set; } = null;
        public Cupones? Cupon { get; set; } = null;

        //ATRIBUTOS DIRECCIÓN ENVIO
        public string? NombrePersonaRecibe { get; set; }
        public string? Calle { get; set; }
        public string? NumeroCasa { get; set; }
        public string? CodigoPostal { get; set; }
        public string? Ciudad { get; set; }
        public string? Estado { get; set; }
        //ATRIBUTOS DIRECCIÓN ENVIO

        //ATRIBUTOS TARJETA
        public string? NombrePersonaTarjeta { get; set; }
        public string? NumeroTarjeta { get; set; }
        public string? MesExpiracion { get; set; }
        public string? AnioExpiracion { get; set; }
        public string? CVV { get; set; }
        //ATRIBUTOS TARJETA

        [JsonIgnore]
        public ApplicationUser? Usuario { get; set; }

        [JsonIgnore]
        public virtual PedidoMayoreo? PedidoMayoreo { get; set; }

        public virtual ICollection<DetalleVenta> DetallesVentas { get; set; }
        public ICollection<SolicitudAsistencia> SolicitudesAsistencia { get; set; }
    }

    public enum MetodoPago
    {
        ContraEntrega = 1,
        TarjetaCredito = 2,
        Stripe = 3,
        Plazos = 4
    }

    public enum MetodoEnvio
    {
        RecogerTienda = 1,
        EnvioDomicilio = 2
    }
    public enum EstatusVenta
    {
        Esperando = 0,
        Recibido = 1,
        Empaquetando = 2,
        Listo = 3,
    }
}

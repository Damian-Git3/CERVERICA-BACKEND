using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string? FullName { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiryTime { get; set; }

        [Required]
        public bool Activo { get; set; }
        public DateTime? FechaRegistro { get; set; } = DateTime.Now;

        // Lista de clientes mayoristas para los que este usuario es agente de ventas
        public ICollection<ClienteMayorista> ClientesMayoristas { get; set; }

        public ICollection<FavoritosComprador> FavoritosComprador { get; set; }

        public ICollection<Cupones> Cupones { get; set; }

        public ICollection<SolicitudesCambioAgente> SolicitudesCambioAgenteActual { get; set; }
        public ICollection<SolicitudesCambioAgente> SolicitudesCambioAgenteNuevo { get; set; }

        public ICollection<SolicitudesPedidoMayoreo> SolicitudesPedidoMayoreo { get; set; }

        public virtual Carrito? Carrito { get; set; }
    }
}

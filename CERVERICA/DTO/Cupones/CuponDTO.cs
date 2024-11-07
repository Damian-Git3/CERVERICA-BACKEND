using CERVERICA.Models;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.DTO.Cupones
{
    public class CuponDTO
    {
        public DateTime FechaCreacion { get; set; }

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

        [Required]
        public decimal MontoMaximo { get; set; }

        [Required]
        public CategoriaComprador CategoriaComprador { get; set; }

        [Required]
        public bool Activo { get; set; }
    }

}

﻿using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class Produccion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        public DateTime FechaProduccion { get; set; }

        public DateTime FechaProximoPaso { get; set; }

        public string Mensaje { get; set; }

        [Required]
        public byte Estatus { get; set; }

        [Required]
        public int NumeroTandas { get; set; }

        public float? LitrosFinales { get; set; }

        public float? CostoProduccion { get; set; }

        [ForeignKey(nameof(Receta))]
        public int IdReceta { get; set; }
        public Receta? Receta { get; set; }

        public DateTime FechaSolicitud { get; set; }

        [ForeignKey(nameof(UsuarioSolicitud))]
        public string IdUsuarioSolicitud { get; set; }
        public virtual ApplicationUser? UsuarioSolicitud { get; set; }

        [ForeignKey(nameof(UsuarioProduccion))]
        public string IdUsuarioProduccion { get; set; }
        public virtual ApplicationUser? UsuarioProduccion { get; set; }
        [ForeignKey(nameof(UsuarioMayorista))]
        public string? IdUsuarioMayorista { get; set; }
        public virtual ApplicationUser? UsuarioMayorista { get; set; }
        [Required]
        public int Paso { get; set; }

        public float? MermaLitros { get; set; }

        public bool? EsMayorista { get; set; }

        public int? IdPedidoMayoreo { get; set; }

        public int? CantidadMayoristaRequerida { get; set; }

        public float? PrecioMayoristaFijado { get; set; }

        public int? StocksRequeridos { get; set; }

        public ICollection<ProduccionLoteInsumo> ProduccionLoteInsumos { get; set; }

    }
}

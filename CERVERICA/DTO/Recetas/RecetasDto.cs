﻿using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class RecetasDto
    {
        public int Id { get; set; }
        [Required]
        public float LitrosEstimados { get; set; }

        public float? PrecioLitro { get; set; }

        public string Descripcion { get; set; }

        [Required]
        [StringLength(100)]
        public string Nombre { get; set; }

        [Required]
        public float CostoProduccion { get; set; }

        [Required]
        public string Imagen { get; set; }

        [Required]
        public bool Activo { get; set; }
    }
}

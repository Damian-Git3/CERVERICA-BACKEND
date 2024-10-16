﻿using CERVERICA.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class RecetaDetallesDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Los litros estimados son obligatorios.")]
        [Range(0, float.MaxValue, ErrorMessage = "Los litros estimados deben ser un valor positivo.")]
        public float LitrosEstimados { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio por litro debe ser un valor positivo.")]
        public float? PrecioLitro { get; set; }

        public string Especificaciones { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 1 debe ser un valor positivo.")]
        public float? PrecioPaquete1 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 6 debe ser un valor positivo.")]
        public float? PrecioPaquete6 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 12 debe ser un valor positivo.")]
        public float? PrecioPaquete12 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 24 debe ser un valor positivo.")]
        public float? PrecioPaquete24 { get; set; }

        [Range(0, 5, ErrorMessage = "La puntuación debe estar entre 0 y 5.")]
        public float? Puntuacion { get; set; }

        public string Descripcion { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "El costo de producción es obligatorio.")]
        [Range(0, float.MaxValue, ErrorMessage = "El costo de producción debe ser un valor positivo.")]
        public float CostoProduccion { get; set; }

        [Required(ErrorMessage = "La imagen es obligatoria.")]
        public string Imagen { get; set; }

        [Range(100, 400, ErrorMessage = "El tiempo de vida debe ser un valor de 100 a 400")]
        public float TiempoVida { get; set; }

        public string RutaFondo { get; set; }

        [Required(ErrorMessage = "El estado activo es obligatorio.")]
        public bool Activo { get; set; }

        public ICollection<IngredienteRecetaDto> IngredientesReceta { get; set; }

        public ICollection<PasosRecetaDto>? PasosReceta { get; set; }
    }
}

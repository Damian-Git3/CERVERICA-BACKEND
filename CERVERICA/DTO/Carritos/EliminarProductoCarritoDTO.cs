﻿using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class EliminarProductoCarritoDTO
    {
        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }

        [Required(ErrorMessage = "La cantidad por lote es obligatoria.")]
        [Range(1, int.MaxValue, ErrorMessage = "La cantidad por lote debe ser al menos 1.")]
        public int CantidadLote { get; set; }
    }
}

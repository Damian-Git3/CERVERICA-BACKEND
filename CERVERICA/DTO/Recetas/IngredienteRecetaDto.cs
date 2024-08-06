﻿using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class IngredienteRecetaDto
    {
        public int IdInsumo { get; set; }
        public float Cantidad { get; set; }
        public string NombreInsumo { get; set; }
        public string UnidadMedida { get; set; }
    }
}

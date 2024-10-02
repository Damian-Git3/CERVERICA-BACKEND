﻿using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class ConsultaSolicitudesOperador
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "El estatus es obligatorio.")]
        public byte Estatus { get; set; }

        [Required(ErrorMessage = "El número de tandas es obligatorio.")]
        public int NumeroTandas { get; set; }

        [Required(ErrorMessage = "El ID de la receta es obligatorio.")]
        public int IdReceta { get; set; }

        [Required(ErrorMessage = "El nombre de la receta es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre de la receta no puede exceder los 100 caracteres.")]
        public string NombreReceta { get; set; }

        [Required(ErrorMessage = "La fecha de solicitud es obligatoria.")]
        public DateTime FechaSolicitud { get; set; }

        [Required(ErrorMessage = "El ID del usuario que solicita es obligatorio.")]
        public string IdUsuarioSolicitud { get; set; }
    }
}

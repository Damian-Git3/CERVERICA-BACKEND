using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class RecetaUpdateDto
    {
        [Required(ErrorMessage = "El ID es obligatorio.")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Los litros estimados son obligatorios.")]
        [Range(0, float.MaxValue, ErrorMessage = "Los litros estimados deben ser un valor positivo.")]
        public float LitrosEstimados { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 1 debe ser un valor positivo.")]
        public float? PrecioPaquete1 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 6 debe ser un valor positivo.")]
        public float? PrecioPaquete6 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 12 debe ser un valor positivo.")]
        public float? PrecioPaquete12 { get; set; }

        [Range(0, float.MaxValue, ErrorMessage = "El precio del paquete de 24 debe ser un valor positivo.")]
        public float? PrecioPaquete24 { get; set; }

        public string Descripcion { get; set; }

        public string Especificaciones { get; set; }

        [Required(ErrorMessage = "El nombre es obligatorio.")]
        [StringLength(100, ErrorMessage = "El nombre no puede exceder los 100 caracteres.")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "La imagen es obligatoria.")]
        public string Imagen { get; set; }

        [Range(100, 400, ErrorMessage = "El tiempo de vida debe ser un valor de 100 a 400")]
        public float TiempoVida { get; set; }

        [Required(ErrorMessage = "La ruta de fondo es obligatoria.")]
        public string RutaFondo { get; set; }

        [Required(ErrorMessage = "La lista de ingredientes es obligatoria.")]
        public List<IngredienteRecetaInsertDto> IngredientesReceta { get; set; }
    }
}

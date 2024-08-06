namespace CERVERICA.Dtos
{
    public class IngredienteRecetaDetalleDto
    {
        public int IdInsumo { get; set; }
        public float Cantidad { get; set; }
        public string NombreInsumo { get; set; }
        public string UnidadMedida { get; set; }
        public float CostoUnitario { get; set; }
    }
}

namespace CERVERICA.Dtos
{
    public class InsumosCantidadDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public string UnidadMedida { get; set; }
        public float CantidadMaxima { get; set; }
        public float CantidadMinima { get; set; }
        public float CostoUnitario { get; set; }
        public float Merma { get; set; }
        public bool Activo { get; set; }
        public float CantidadTotalLotes { get; set; }
    }
}

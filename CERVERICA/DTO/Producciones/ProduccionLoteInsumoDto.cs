namespace CERVERICA.Dtos
{
    public class ProduccionLoteInsumoDto
    {
        public int Id { get; set; }
        public float Cantidad { get; set; }
        public int IdLoteInsumo { get; set; }
        public int IdInsumo { get; set; }
        public string NombreInsumo { get; set; }
        public string UnidadMedida { get; set; }
    }
}

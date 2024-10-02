namespace CERVERICA.Dtos
{
    public class RecetaConStockDto
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Descripcion { get; set; }
        public float LitrosEstimados { get; set; }
        public float? PrecioLitro { get; set; }
        public float? PrecioPaquete1 { get; set; }
        public float? PrecioPaquete6 { get; set; }
        public float? PrecioPaquete12 { get; set; }
        public float? PrecioPaquete24 { get; set; }
        public int? LotesMinimos { get; set; }
        public int? LotesMaximos { get; set; }

        public float CostoProduccion { get; set; }
        public float CantidadEnStock { get; set; }
    }
}
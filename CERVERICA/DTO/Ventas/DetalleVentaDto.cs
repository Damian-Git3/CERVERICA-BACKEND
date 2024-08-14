namespace CERVERICA.Dtos
{
    public class DetalleVentaDto
    {
        public int IdReceta { get; set; }
        public int Cantidad { get; set; }
        public int Pack { get; set; }
        public string? TipoEnvase { get; set; } = "Botella";
        public int? MedidaEnvase { get; set; } = 355;
    }
}

namespace CERVERICA.Dtos
{
    public class FinalizarProduccionDto
    {
        public float? LitrosFinales { get; set; }
        public float? MermaLitros { get; set; }
        public string? TipoEnvase { get; set; } = "Botella";
        public int? MedidaEnvase { get; set; } = 355;
        public string Mensaje { get; set; }
        public bool? CalcularMerma { get; set; }
        public bool? ProduccionFallida { get; set; }

    }
}

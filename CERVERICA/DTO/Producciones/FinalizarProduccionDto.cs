namespace CERVERICA.Dtos
{
    public class FinalizarProduccionDto
    {
        public float? LitrosFinales { get; set; }
        public float? MermaLitros { get; set; }
        public string TipoEnvase { get; set; }
        public int? MedidaEnvase { get; set; }  
        public string Mensaje { get; set; }
        public bool? CalcularMerma { get; set; } = false;
        public bool? ProduccionFallida { get; set; } = false;

    }
}

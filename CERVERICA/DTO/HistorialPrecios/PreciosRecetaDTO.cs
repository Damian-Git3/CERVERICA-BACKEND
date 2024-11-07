namespace CERVERICA.DTO.HistorialPrecios
{
    public class PreciosRecetaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string Imagen { get; set; }
        public float PrecioLitro { get; set; }
        public float PrecioPaquet1 { get; set; }
        public float PrecioPaquete6 { get; set; }
        public float PrecioPaquete12 { get; set; }
        public float PrecioPaquete24 { get; set; }
        public bool Estatus { get; set; }

    }
}

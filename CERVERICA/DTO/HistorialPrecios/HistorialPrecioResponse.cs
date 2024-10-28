using CERVERICA.Models;

namespace CERVERICA.Dtos
{
    public class HistorialPrecioResponse(HistorialPrecios hp)
    {
        public int Id { get; set; } = hp.Id;
        public int IdReceta { get; set; } = hp.IdReceta;
        public DateTime Fecha { get; set; } = hp.Fecha;
        public double Paquete1 { get; set; } = hp.Paquete1;
        public double Paquete6 { get; set; } = hp.Paquete6;
        public double Paquete12 { get; set; } = hp.Paquete12;
        public double Paquete24 { get; set; } = hp.Paquete24;
        public double CostoProduccionUnidad { get; set; } = hp.CostoProduccionUnidad;
        public double PrecioUnitarioMinimoMayoreo { get; set; } = hp.PrecioUnitarioMinimoMayoreo;
        public double PrecioUnitarioBaseMayoreo { get; set; } = hp.PrecioUnitarioBaseMayoreo;
    }
}

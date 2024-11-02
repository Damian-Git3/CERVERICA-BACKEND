namespace CERVERICA.DTO.Agente
{
    public class SolicitudCambioAgenteResponseDTO
    {
        public int Id { get; set; }
        public string Motivo { get; set; }
        public int Solicitante { get; set; }
        public string Estatus { get; set; }
        public DateTime FechaSolicitud { get; set; }
        public DateTime? FechaRespuesta { get; set; }
        public string MotivoRechazo { get; set; }

        // DATOS CLIENTE
        public int IdMayorista { get; set; }
        //public string IdCliente { get; set; }
        public string NombreContacto { get; set; }
        public string CargoContacto { get; set; }
        public string TelefonoContacto { get; set; }
        public string EmailContacto { get; set; }

        // DATOS AGENTES
        public string IdAgenteVentaActual { get; set; }
        public string AgenteVentaActualNombre { get; set; } // Nombre del agente actual
        public string IdAgenteVentaNuevo { get; set; }
        public string AgenteVentaNuevoNombre { get; set; } // Nombre del nuevo agente (puede ser null)

        // DATOS ADMINISTRADOR
        public string IdAdministrador { get; set; }
        public string AdministradorNombre { get; set; }
    }


}

public class UserMayoristaDetailDto
{
    // Datos del usuario
    public string Id { get; set; }
    public string Email { get; set; }
    public string FullName { get; set; }
    public IList<string> Roles { get; set; }
    public string PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public int AccessFailedCount { get; set; }
    public bool Activo { get; set; }

    // Datos del cliente mayorista
    public int IdMayorista { get; set; }
    public string RFCEmpresa { get; set; }
    public string NombreEmpresa { get; set; }
    public string DireccionEmpresa { get; set; }
    public string TelefonoEmpresa { get; set; }
    public string EmailEmpresa { get; set; }
    public string NombreContacto { get; set; }
    public string CargoContacto { get; set; }
    public string TelefonoContacto { get; set; }
    public string EmailContacto { get; set; }

    // Datos del agente de ventas
    public AgenteVentaDto AgenteVenta { get; set; }
}

// DTO para los detalles del agente de ventas
public class AgenteVentaDto
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
}

/*
// Ejemplo del DTO para una solicitud de cambio de agente
public class SolicitudCambioAgenteDto
{
    public int Id { get; set; }
    public DateTime FechaSolicitud { get; set; }
    public string EstadoSolicitud { get; set; }
    // Otros campos relevantes
}

// Ejemplo del DTO para un pedido de mayoreo
public class PedidoMayoreoDto
{
    public int Id { get; set; }
    public DateTime FechaPedido { get; set; }
    public decimal TotalPedido { get; set; }
    // Otros campos relevantes
}
*/
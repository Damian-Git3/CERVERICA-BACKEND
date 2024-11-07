using CERVERICA.DTO.Usuarios;
using CERVERICA.Models;
using Microsoft.AspNetCore.Mvc;

namespace CERVERICA.Dtos
{
    public class SolicitudAsistenciaDTO
    {
        public int IdCategoriaAsistencia { get; set; }
        public bool Mayoreo { get; set; }
        public string Descripcion { get; set; }
        public int Tipo { get; set; }

    }

    public class CrearCategoriaDTO
    {
        public string Nombre { get; set; }
    }

    public class SeleccionarCategoriaDTO
    {
        public int Id { get; set; }
    }

    public class SeleccionarClienteDTO
    {
        public string Id { get; set; }
    }

    public class SeguimientoSolicitudAsistenciaDTO
    {
        public int IdSolicitudAsistencia { get; set; }
        public string Descripcion { get; set; }
        public string Mensaje { get; set; }
    }

    public class CerrarSolicitudAsistenciaDTO
    {
        public int IdSolicitudAsistencia { get; set; }
        public string Descripcion { get; set; }
    }

    public class CategoriaAsistenciaDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public bool Estatus { get; set; }
    }
    public class SolicitudValoracionDTO
    {
        public int IdSolicitudAsistencia { get; set; }
        public float Valoracion { get; set; }
        public string Mensaje { get; set; }
    }

    public class ReasignarAgenteDTO
    {
        public int IdSolicitudAsistencia { get; set; }
        public string IdAgenteVenta { get; set; }
        
    }

}

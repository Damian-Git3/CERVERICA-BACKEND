using CERVERICA.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Dtos
{
    public class ConsultaSolicitudesOperador
    {
        public int Id { get; set; }

        public byte Estatus { get; set; }

        public int NumeroTandas { get; set; }


        public int IdReceta { get; set; }

        public string NombreReceta { get; set; }

        public DateTime FechaSolicitud { get; set; }

        public string IdUsuarioSolicitud { get; set; }
    }
}

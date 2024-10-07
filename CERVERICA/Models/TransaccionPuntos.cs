
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CERVERICA.Models
{
    public class TransaccionPuntos
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        // Foreign key hacia el usuario
        [ForeignKey("Usuario")]
        public string IdUsuario { get; set; }
        public virtual ApplicationUser? Usuario { get; set; }

        // Puntos ganados (positivo) o redimidos (negativo)
        public int Puntos { get; set; }

        // Tipo de transacción: GANO o REDIMIO
        public string TipoTransaccion { get; set; }

        public DateTime FechaTransaccion { get; set; }

        // Descripción de la transacción, p.ej. "Compra de $100"
        public string Descripcion { get; set; }


    }
}
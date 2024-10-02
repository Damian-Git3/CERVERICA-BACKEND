namespace CERVERICA.DTO.Comentarios
{
    public class ComentarioDTO
    {
        public int Id { get; set; }
        public float Puntuacion { get; set; }
        public string TextoComentario { get; set; }
        public string NombreUsuario { get; set; }
        public DateTime Fecha { get; set; }
        public string IdUsuario { get; set; }
        public int IdReceta { get; set; }
    }
}

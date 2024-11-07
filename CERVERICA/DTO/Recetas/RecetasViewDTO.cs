namespace CERVERICA.DTO.Recetas
{
    public class RecetasViewDTO
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public float Precio { get; set; }
        public string Imagen { get; set; }

        public bool Activo { get; set; }
    }
}

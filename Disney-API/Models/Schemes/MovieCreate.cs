using Disney_API.Models.Interfaces;

namespace Disney_API.Models.Schemes
{
    public class MovieCreate : IMovie
    {
        public string Imagen { get; set; } = null!;
        public string Titulo { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public float Calificacion { get; set; }
    }
}

namespace Disney_API.Models.Interfaces
{
    public interface IMovie
    {
        public string Imagen { get; set; }
        public string Titulo { get; set; } 
        public DateTime Fecha { get; set; }
        public float Calificacion { get; set; }
    }

  
}

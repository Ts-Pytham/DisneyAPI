using System;
using System.Collections.Generic;
using Disney_API.Models.Interfaces;
namespace Disney_API.Models
{
    public partial class Pelicula : IMovie
    {
        public int Idpelicula { get; set; }
        public string Imagen { get; set; } = null!;
        public string Titulo { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public double Calificacion { get; set; }

        public static implicit operator Pelicula(MovieCreate v)
        {
            return new MovieCreate
            {
                Imagen = v.Imagen,
                Titulo = v.Titulo,
                Fecha = v.Fecha,
                Calificacion = v.Calificacion
            };
        }
    }


    public class MovieCreate : IMovie
    {
        public string Imagen { get; set; } = null!;
        public string Titulo { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public double Calificacion { get; set; }
    }

    public class MovieUpdate : MovieCreate
    {

    }
}

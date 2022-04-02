using System;
using System.Collections.Generic;

namespace Disney_API.Models
{
    public partial class Pelicula
    {
        public int Idpelicula { get; set; }
        public string Imagen { get; set; } = null!;
        public string Titulo { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public double Calificacion { get; set; }
    }
}

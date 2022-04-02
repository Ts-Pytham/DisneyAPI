using System;
using System.Collections.Generic;

namespace Disney_API.Models
{
    public partial class GeneroPelicula
    {
        public int Idgenero { get; set; }
        public int Idpelicula { get; set; }

        public virtual Genero IdgeneroNavigation { get; set; } = null!;
        public virtual Pelicula IdpeliculaNavigation { get; set; } = null!;
    }
}
